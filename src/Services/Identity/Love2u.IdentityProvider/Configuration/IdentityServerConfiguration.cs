using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace Love2u.IdentityProvider.Configuration
{
    internal class IdentityServerConfiguration
    {
        internal static IEnumerable<Client> Clients(IConfiguration configuration) => new Client[]
        {
            new Client
            {
                ClientId = "Postman",
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                ClientName = "Postman - developer client",
                ClientSecrets = { new Secret("H!lde_1984".Sha256()) },
                AllowedScopes = { StandardScopes.OpenId, StandardScopes.Profile, StandardScopes.Phone, StandardScopes.Email,
                    StandardScopes.Address, StandardScopes.OfflineAccess },
                RedirectUris = new[] { "https://localhost:8000/signin-oidc" },
                PostLogoutRedirectUris = new[] { "https://localhost:8000/signout-oidc" },
                RequireConsent = false
            },
            new Client
            {
                ClientId = "Love2uAngular",
                AllowedGrantTypes = GrantTypes.Code,
                ClientName = "Love2u - Angular frontend",
                AllowedScopes = { StandardScopes.OpenId, StandardScopes.Profile, StandardScopes.Phone, StandardScopes.Email,
                    StandardScopes.Address, StandardScopes.OfflineAccess, "profiles.api" },
                RedirectUris = new[] { $"{configuration["ANGULAR_SPA_ORIGIN"]}/authentication/login-callback" },
                PostLogoutRedirectUris = new[] { $"{configuration["ANGULAR_SPA_ORIGIN"]}/authentication/logout-callback" },
                RequireConsent = false,
                RequirePkce = true,
                AllowedCorsOrigins = new[] { configuration["ANGULAR_SPA_ORIGIN"] },
                AllowOfflineAccess = true,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                RequireClientSecret = false,
                RefreshTokenUsage = TokenUsage.ReUse,
                UpdateAccessTokenClaimsOnRefresh = true
            }
        };

        internal static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Phone(),
            new IdentityResources.Email(),
            new IdentityResources.Address()
        };

        internal static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("profiles.api", "User profiles API")
        };
    }
}
