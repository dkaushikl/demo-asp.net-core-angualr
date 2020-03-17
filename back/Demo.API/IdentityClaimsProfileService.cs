namespace Demo.API
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Demo.API.Models;

    using IdentityModel;

    using IdentityServer4;
    using IdentityServer4.Extensions;
    using IdentityServer4.Models;
    using IdentityServer4.Services;

    using Microsoft.AspNetCore.Identity;

    public class IdentityClaimsProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;

        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityClaimsProfileService(
            UserManager<ApplicationUser> userManager,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
        {
            this._userManager = userManager;
            this._claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await this._userManager.FindByIdAsync(sub);
            var principal = await this._claimsFactory.CreateAsync(user);
            var roles = await this._userManager.GetRolesAsync(user);
            var claims = principal.Claims.ToList();
            claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
            claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
            claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));

            // foreach (string role in roles)
            // {
            claims.Add(new Claim(JwtClaimTypes.Role, Convert.ToString(roles.FirstOrDefault())));

            // }
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await this._userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}