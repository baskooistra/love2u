using Love2u.Profiles.Domain.Models.Aggregates.UserProfile;
using Love2u.Profiles.Domain.Services;
using Love2u.Profiles.InfraStructure.CosmosDB;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.InfraStructure.Extensions
{
    public record CosmosDBOptions(string Uri, string Key, string DatabaseName);
    
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) 
        {
            services.Configure<CosmosDBOptions>(configure => configuration.GetSection("CosmosDB").Bind(configure));
            services.AddSingleton<IDataStore<UserProfile>>(CosmosDBStoreFactory.Create<UserProfile>);

            return services;
        }
    }
}
