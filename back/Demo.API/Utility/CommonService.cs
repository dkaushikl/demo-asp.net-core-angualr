namespace Demo.API.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Demo.API.Infrastructure;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;

    using Newtonsoft.Json;

    public class CommonService : ICommonService
    {
        private readonly IConfiguration _iconfiguration;

        public CommonService(IConfiguration iconfiguration)
        {
            this._iconfiguration = iconfiguration;
        }

        /// <summary>
        ///     Generates a Random Password
        ///     respecting the given strength requirements.
        /// </summary>
        /// <param name="opts">
        ///     A valid PasswordOptions object
        ///     containing the password strength requirements.
        /// </param>
        /// <returns>A random password</returns>
        public string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null)
                opts = new PasswordOptions
                           {
                               RequiredLength = 8,
                               RequireDigit = true,
                               RequireLowercase = true,
                               RequireNonAlphanumeric = true,
                               RequireUppercase = true
                           };

            string[] randomChars =
                {
                    "ABCDEFGHJKLMNOPQRSTUVWXYZ", // uppercase 
                    "abcdefghijkmnopqrstuvwxyz", // lowercase
                    "0123456789", // digits
                    "!@$?_-" // non-alphanumeric
                };

            var rand = new Random(Environment.TickCount);
            var chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count), randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count), randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count), randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count), randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (var i = chars.Count;
                 i < opts.RequiredLength || chars.Distinct().Count() < opts.RequiredUniqueChars;
                 i++)
            {
                var rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count), rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        public async Task<string> GenerateToken(string email, string password)
        {
            var client = new HttpClient { BaseAddress = new Uri(this._iconfiguration["Identity:Authority"]) };

            var parameters = new Dictionary<string, string>
                                 {
                                     { "grant_type", "password" },
                                     { "scope", "openid api1" },
                                     { "client_id", "rel.angular" },
                                     { "client_secret", "secret" },
                                     { "username", string.Empty + email + string.Empty },
                                     { "password", string.Empty + password + string.Empty }
                                 };

            var response = await client.PostAsync("/connect/token", HttpClientHelpers.GetPostBody(parameters));
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenDetail>(responseBody);
            return token.access_token;
        }
    }
}