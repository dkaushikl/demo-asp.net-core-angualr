namespace Demo.API
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Reflection;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Demo.API.Data;
    using Demo.API.Infrastructure;
    using Demo.API.Models;
    using Demo.API.Utility;

    using IdentityServer4.Services;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using NJsonSchema;

    using NSwag.AspNetCore;

    using Serilog;
    using Serilog.Events;
    using Serilog.Sinks.MSSqlServer;

    using StructureMap;

    public class Startup
    {
        private readonly IHostingEnvironment _env;

        private readonly int? _sslPort;

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            this._env = env;

            this.Configuration = configuration;

            if (env.IsDevelopment())
            {
                var launchConfiguration = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                    .AddJsonFile(@"Properties\launchSettings.json").Build();

                // During development we won't be using port 443
                this._sslPort = launchConfiguration.GetValue<int>("iisSettings:iisExpress:sslPort");
            }

            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true).AddEnvironmentVariables();

            this.Configuration = builder.Build();

            var columnOptions = new ColumnOptions
            {
                AdditionalDataColumns = new Collection<DataColumn>
                                                                    {
                                                                        new DataColumn
                                                                            {
                                                                                DataType = typeof(string),
                                                                                ColumnName = "User"
                                                                            },
                                                                        new DataColumn
                                                                            {
                                                                                DataType = typeof(string),
                                                                                ColumnName = "Other"
                                                                            }
                                                                    }
            };

            columnOptions.Store.Add(StandardColumn.LogEvent);

            Log.Logger = new LoggerConfiguration().MinimumLevel.Information().MinimumLevel
                .Override("Microsoft", LogEventLevel.Warning).MinimumLevel.Override("System", LogEventLevel.Error)
                .WriteTo.MSSqlServer(
                    this.Configuration["Serilog:ConnectionString"],
                    this.Configuration["Serilog:TableName"],
                    LogEventLevel.Information,
                    columnOptions: columnOptions).CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            else app.UseExceptionHandler("/home/error");

            app.UseStaticFiles();

            app.UseSwaggerUi(
                typeof(Startup).GetTypeInfo().Assembly,
                settings =>
                    {
                        settings.GeneratorSettings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;
                    });
            app.UseStatusCodePages();

            app.UseIdentityServer();

            app.UseAuthentication();

            app.UseDefaultFiles();

            app.UseCors("allowAngular");

            app.UseMvc();

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();
            }

            this.CreateUsersAndRolesAndCheckList(serviceProvider).Wait();

            app.UseWelcomePage();
        }

        public void ConfigureIoC(IServiceCollection services, Container container)
        {
            container.Configure(
                config =>
                    {
                        config.AddRegistry(new StandardRegistry());
                        config.For<UserManager<ApplicationUser>>().Use<UserManager<ApplicationUser>>();
                        config.For<SignInManager<ApplicationUser>>().Use<SignInManager<ApplicationUser>>();
                        config.Populate(services);
                    });
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            services.AddStructureMap();

            var connectionString = this.Configuration.GetConnectionString("DemoConnection");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>(
                opts =>
                    {
                        opts.Password.RequireDigit = true;
                        opts.Password.RequireLowercase = true;
                        opts.Password.RequireUppercase = true;
                        opts.Password.RequireNonAlphanumeric = false;
                        opts.Password.RequiredLength = 8;
                    }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddTransient<IProfileService, IdentityClaimsProfileService>();
            services.Configure<EmailConfig>(this.Configuration.GetSection("Email"));

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ICommonService, CommonService>();

            services.AddIdentityServer().AddDeveloperSigningCredential().AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources()).AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<ApplicationUser>();

            services.AddMvc(
                options =>
                    {
                        options.Filters.Add(new RequireHttpsAttribute());
                        options.SslPort = this._sslPort;
                    });

            services.AddAuthentication(
                options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    }).AddJwtBearer(
                options =>
                    {
                        // base-address of your identityserver
                        options.Authority = this.Configuration["Identity:Authority"];

                        // name of the API resource
                        options.Audience = "api1";

                        options.RequireHttpsMetadata = true;
                    });

            services.AddAuthorization(
                options =>
                    {
                        options.AddPolicy(
                            RolesNames.SuperAdmin,
                            policy => policy.RequireClaim("role", RolesNames.SuperAdmin));
                        options.AddPolicy(RolesNames.User, policy => policy.RequireClaim("role", RolesNames.User));
                    });

            services.AddCors(
                cors =>
                    {
                        cors.AddPolicy(
                            "allowAngular",
                            policyBuilder =>
                                {
                                    policyBuilder.AllowAnyMethod().AllowAnyHeader()
                                        .WithOrigins(this.Configuration["Cors:AllowedOrigin"]);
                                });
                    });

            services.AddSingleton(this.Configuration);

            var container = new Container();

            services.AddSingleton<IStructureMapContainer, StructureMapContainer>(
                ctx => new StructureMapContainer(container));

            this.ConfigureIoC(services, container);

            return container.GetInstance<IServiceProvider>();
        }

        private async Task CreateUsersAndRolesAndCheckList(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = { RolesNames.SuperAdmin, RolesNames.User };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var superadmin = await userManager.FindByEmailAsync("superadmin@email.com");

            if (superadmin == null)
            {
                var poweruser = new ApplicationUser
                {
                    UserName = "superadmin@email.com",
                    Email = "superadmin@email.com",
                    FirstName = "Master",
                    LastName = "Admin",
                    Disabled = false
                };
                var adminPassword = "Password123#";

                var createPowerUser = await userManager.CreateAsync(poweruser, adminPassword);
                if (createPowerUser.Succeeded)
                {
                    poweruser.EmailConfirmed = true;
                    await userManager.UpdateAsync(poweruser);
                    await userManager.AddToRoleAsync(poweruser, RolesNames.SuperAdmin);

                    await userManager.AddClaimAsync(poweruser, new Claim("userName", poweruser.UserName));
                    await userManager.AddClaimAsync(poweruser, new Claim("firstName", poweruser.FirstName));
                    await userManager.AddClaimAsync(poweruser, new Claim("lastName", poweruser.LastName));
                    await userManager.AddClaimAsync(poweruser, new Claim("email", poweruser.Email));
                }
            }

            var user = await userManager.FindByEmailAsync("dhameliyakaushik13@gmail.com");

            if (user == null)
            {
                var poweruser = new ApplicationUser
                {
                    UserName = "dhameliyakaushik13@email.com",
                    Email = "dhameliyakaushik13@email.com",
                    FirstName = "Kaushik",
                    LastName = "Dhameliya",
                    Disabled = false
                };
                var adminPassword = "Password123#";

                var createPowerUser = await userManager.CreateAsync(poweruser, adminPassword);
                if (createPowerUser.Succeeded)
                {
                    poweruser.EmailConfirmed = true;
                    await userManager.UpdateAsync(poweruser);
                    await userManager.AddToRoleAsync(poweruser, RolesNames.User);

                    await userManager.AddClaimAsync(poweruser, new Claim("userName", poweruser.UserName));
                    await userManager.AddClaimAsync(poweruser, new Claim("firstName", poweruser.FirstName));
                    await userManager.AddClaimAsync(poweruser, new Claim("lastName", poweruser.LastName));
                    await userManager.AddClaimAsync(poweruser, new Claim("email", poweruser.Email));
                }
            }
        }
    }
}