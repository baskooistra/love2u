using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Configuration;
using RabbitMQ.Client;

namespace Love2u.Profiles.InfraStructure.RabbitMQ
{
    internal class RabbitMQConnectionFactory
    {
        internal static IModel Create(IServiceProvider serviceProvider)
        {
            Log.Debug($"Initializing RabbitMQ connection...");

            var options = serviceProvider.GetService<IOptions<RabbitMQOptions>>().Value;

            if (!ValidateOptions(options))
            {
                Log.Fatal("Invalid RabbitMQ configuration. Initialization failed.");
                throw new ConfigurationErrorsException("Invalid RabbitMQ configuration during initialization.");
            }

            return Initialize(options);
        }

        private static IModel Initialize(RabbitMQOptions options)
        {
            var connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = options.HostName;
            connectionFactory.Port = options.PortNumber;
            connectionFactory.UserName = options.UserName;
            connectionFactory.Password = options.Password;

            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            return channel;
        }

        private static bool ValidateOptions(RabbitMQOptions options)
        {
            if (options == null)
                return false;

            if (string.IsNullOrWhiteSpace(options.HostName) || string.IsNullOrWhiteSpace(options.UserName) ||
                string.IsNullOrWhiteSpace(options.Password) || !(options.PortNumber > 0))
                return false;

            return true;
        }
    }
}
