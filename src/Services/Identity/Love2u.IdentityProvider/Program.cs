using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Love2u.IdentityProvider.Data.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Love2u.IdentityProvider
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    await SeedData.EnsureSeedDataAsync(services);
                }
                catch (Exception exc)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exc, "An error occurred while migrating and seeding the database.");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
