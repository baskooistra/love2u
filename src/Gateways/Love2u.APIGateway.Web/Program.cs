using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System;
using System.IO;
using System.Reflection;

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
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                    optional: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.Elasticsearch(ConfigureElasticSink())
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

        private static ElasticsearchSinkOptions ConfigureElasticSink()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var host = Environment.GetEnvironmentVariable("ELASTICSEARCH_HOSTS");

            if (string.IsNullOrWhiteSpace(host)) throw new ArgumentNullException($"Invalid environment variable 'ELASTICSEARCH_HOSTS'.");

            return new ElasticsearchSinkOptions(new Uri(host))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                MinimumLogEventLevel = LogEventLevel.Information,
                EmitEventFailure = EmitEventFailureHandling.ThrowException
            };
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
                            
                            services.AddOcelot(Configuration);
                        })
                        .ConfigureLogging((hostingContext, logging) =>
                        {
                            logging.AddSerilog(logger: Log.Logger);
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
                .UseSerilog(logger: Log.Logger);
        
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
                        ValidAudiences = new[] { "profiles.api" },
                        ValidIssuers = new[] { Configuration["IDENTITY_PROVIDER_URL"], Configuration["IDENTITY_PROVIDER_EXTERNAL"] }
                    };
                });
        }
    }
}
