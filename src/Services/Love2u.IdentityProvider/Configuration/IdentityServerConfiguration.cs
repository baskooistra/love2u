using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace Love2u.IdentityProvider.Configuration
{
    internal class IdentityServerConfiguration
    {
        internal static IEnumerable<Client> Clients => new Client[]
        {
            new Client
            {
                ClientId = "Postman",
                AllowedGrantTypes = { GrantType.ResourceOwnerPassword, GrantType.Implicit, GrantType.Implicit, GrantType.DeviceFlow,
                    GrantType.ClientCredentials, GrantType.AuthorizationCode },
                ClientName = "Postman - developer client",
                ClientSecrets = { new Secret("H!lde_1984".Sha256()) },
                AllowedScopes = { StandardScopes.OpenId, StandardScopes.Profile, StandardScopes.Phone, StandardScopes.Email,
                    StandardScopes.Address, StandardScopes.OfflineAccess }
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
    }
}
