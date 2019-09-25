using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using Love2u.IdentityProvider.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Love2u.IdentityProvider.Data.Seed
{
    internal class SeedData
    {
        internal static async Task EnsureSeedDataAsync(IServiceProvider serviceProvider)
        {
            await PerformMigrationsAsync(serviceProvider);
            await SeedIdentityServerConfigurationAsync(serviceProvider.GetRequiredService<ConfigurationDbContext>());
        }

        private static async Task PerformMigrationsAsync(IServiceProvider serviceProvider)
        {
            await serviceProvider.GetRequiredService<Love2uIdentityContext>().Database.MigrateAsync();
            await serviceProvider.GetRequiredService<ConfigurationDbContext>().Database.MigrateAsync();
            await serviceProvider.GetRequiredService<PersistedGrantDbContext>().Database.MigrateAsync();
        }

        private static async Task SeedIdentityServerConfigurationAsync(IConfigurationDbContext configuration)
        {
            // Seed OIDC clients from configuration
            var clients = IdentityServerConfiguration.Clients;
            foreach (var client in clients)
            {
                var clientEntity = configuration.Clients.SingleOrDefaultAsync(c => c.ClientId == client.ClientId);
                if (clientEntity == null)
                {
                    configuration.Clients.Add(client.ToEntity());
                }
                else
                {
                    configuration.Clients.Update()
                }
            }

            // Seed OIDC resource scopes from configuration
            var identityResources = IdentityServerConfiguration.IdentityResources;
            foreach (var identityResource in identityResources)
            {
                if (!configuration.IdentityResources.Any(c => c.Name == identityResource.Name))
                {
                    configuration.IdentityResources.Add(identityResource.ToEntity());
                }
            }

            await configuration.SaveChangesAsync();
        }
    }
}
