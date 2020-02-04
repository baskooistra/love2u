using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using Love2u.IdentityProvider.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
            await SeedIdentityServerConfigurationAsync(serviceProvider.GetRequiredService<ConfigurationDbContext>(),
                serviceProvider.GetRequiredService<IConfiguration>());
        }

        private static async Task PerformMigrationsAsync(IServiceProvider serviceProvider)
        {
            await serviceProvider.GetRequiredService<Love2uIdentityContext>().Database.MigrateAsync();
            await serviceProvider.GetRequiredService<ConfigurationDbContext>().Database.MigrateAsync();
            await serviceProvider.GetRequiredService<PersistedGrantDbContext>().Database.MigrateAsync();
        }

        private static async Task SeedIdentityServerConfigurationAsync(IConfigurationDbContext context, IConfiguration configuration)
        {
            // Seed OIDC clients from configuration
            var clients = IdentityServerConfiguration.Clients(configuration);
            foreach (var client in clients)
            {
                var clientEntity = await context.Clients.SingleOrDefaultAsync(c => c.ClientId == client.ClientId);
                if (clientEntity == null)
                {
                    context.Clients.Add(client.ToEntity());
                }
            }

            // Seed OIDC resource scopes from configuration
            var identityResources = IdentityServerConfiguration.IdentityResources;
            foreach (var identityResource in identityResources)
            {
                if (!context.IdentityResources.Any(c => c.Name == identityResource.Name))
                {
                    context.IdentityResources.Add(identityResource.ToEntity());
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
