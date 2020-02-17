using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityServer4;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiResource> Apis =>
            new[]
            {
                new ApiResource("api1", "My API")
            };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                /*
                    You can think of the ClientId and the ClientSecret as the login and password for your application itself. 
                    It identifies your application to the identity server so that it knows which application is trying to connect to it.
                 */
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientId/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedScopes = {"api1"}
                },
                new Client
                {
                    ClientId = "mvcclient",
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = true,

                    // Where to redirect to after login
                    RedirectUris = {"http://localhost:5002/signin-oidc"},

                    // Where to redirect to after logout
                    PostLogoutRedirectUris = {"http://localhost:5002/signout-callback-oidc"},

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },

                    // Enable support for refresh tokens
                    AllowOfflineAccess = true
                }
            };
    }
}