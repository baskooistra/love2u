using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace Love2u.Profiles.API
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Debug(LogEventLevel.Debug)
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.Elasticsearch(ConfigureElasticSink())
                .CreateLogger();

            try
            {
                Log.Information("Starting profile API...");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception exc)
            {
                Log.Fatal(exc, "Error starting profile API");
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
                    .UseStartup<Startup>();
            }).UseSerilog();
    }
}
