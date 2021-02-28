using Love2u.Profiles.Domain.Models.Aggregates.UserProfile;
using Love2u.Profiles.Domain.Services;
using Love2u.Profiles.InfraStructure.CosmosDB;
using Love2u.Profiles.InfraStructure.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Love2u.Profiles.InfraStructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) 
        {
            // Add Cosmos DB services
            services.Configure<CosmosDBOptions>(configure => configuration.GetSection("CosmosDB").Bind(configure));
            services.AddSingleton<IDataStore<UserProfile>>(CosmosDBStoreFactory.Create<UserProfile>);

            // Add RabbitMQ services
            services.Configure<RabbitMQOptions>(configure => configuration.GetSection("RabbitMQ").Bind(configure));
            services.AddScoped(RabbitMQConnectionFactory.Create);
            services.AddScoped<RabbitMQMessageBroker>();

            return services;
        }
    }
}
