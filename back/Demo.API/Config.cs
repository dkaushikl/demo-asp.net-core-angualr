namespace Demo.API
{
    using System.Collections.Generic;
    using System.Security.Claims;

    using IdentityModel;

    using IdentityServer4;
    using IdentityServer4.Models;

    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource> { new ApiResource("api1") { UserClaims = { "role" } } };
        }

        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
                       {
                           // resource owner password grant client
                           new Client
                               {
                                   ClientId = "rel.angular",
                                   AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                                   ClientSecrets = {
                                                      new Secret("secret".Sha256()) 
                                                   },
                                   AllowedScopes =
                                       {
                                           IdentityServerConstants.StandardScopes.OpenId,
                                           IdentityServerConstants.StandardScopes.Profile,
                                           IdentityServerConstants.StandardScopes.Email,
                                           IdentityServerConstants.StandardScopes.Address,
                                           "api1",
                                           "roles"
                                       }
                               }
                       };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
                       {
                           new IdentityResources.OpenId(),
                           new IdentityResources.Email(),
                           new IdentityResources.Profile(),
                           new IdentityResource
                               {
                                   Name = "roles",
                                   DisplayName = "Roles",
                                   UserClaims = new[] { JwtClaimTypes.Role, ClaimTypes.Role },
                                   ShowInDiscoveryDocument = true,
                                   Required = true,
                                   Emphasize = true
                               }
                       };
        }
    }
}