using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

namespace Love2u.APIGateway.Web
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddJsonFile("ocelot-configuration.json")
            .AddEnvironmentVariables()
            .Build();

        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                Log.Information("Starting ocelot API gateway for web...");

                CreateHostBuilder(args).Build().Run();

                return 0;
            }
            catch (Exception exc)
            {
                Log.Fatal(exc, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseKestrel()
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseConfiguration(Configuration)
                        .ConfigureServices(services =>
                        {
                            AddAuthenticationMiddleware(services);
                            services.AddCors(options =>
                            {
                                options.AddPolicy("Love2u.Angular", policyBuilder =>
                                {
                                    policyBuilder.WithOrigins(Configuration["ANGULAR_SPA_ORIGIN"])
                                        .AllowAnyMethod()
                                        .AllowAnyHeader();
                                });
                            });
                            
                            services.AddOcelot();
                        })
                        .ConfigureLogging((hostingContext, logging) =>
                        {
                            logging.AddSerilog(dispose: true);
                        })
                        .UseIIS()
                        .Configure(app =>
                        {
                            app.UseSerilogRequestLogging();
                            app.UseCors("Love2u.Angular");
                            app.UseAuthentication();
                            app.UseOcelot().Wait();
                        });
                })
            .UseSerilog();
        
        private static void AddAuthenticationMiddleware(IServiceCollection services)
        {
            string identityProviderUrl = Configuration["IDENTITY_PROVIDER_URL"];
            string authenticationProviderKey = "Love2uIdentityKey";

            if (Configuration["ASPNETCORE_ENVIRONMENT"] == "Development") 
            {
                IdentityModelEventSource.ShowPII = true;
            }

            services.AddAuthentication()
                .AddJwtBearer(authenticationProviderKey, options =>
                {
                    options.Authority = identityProviderUrl;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudiences = new[] { "profile.api" },
                        ValidIssuers = new[] { Configuration["IDENTITY_PROVIDER_URL"], Configuration["IDENTITY_PROVIDER_EXTERNAL"] }
                    };
                });
        }
    }
}
