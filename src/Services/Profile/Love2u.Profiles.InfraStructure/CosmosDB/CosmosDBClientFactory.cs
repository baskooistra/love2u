using Azure;
using Love2u.Profiles.Domain.Models;
using Love2u.Profiles.Domain.Services;
using Love2u.Profiles.InfraStructure.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Love2u.Profiles.InfraStructure.CosmosDB
{
    internal static class CosmosDBStoreFactory
    {
        internal static CosmosDBStore<T> Create<T>(IServiceProvider serviceProvider) where T : Entity, IAggregateRoot 
        {
            Log.Debug($"Creating CosmosDB Store for entity {typeof(T).Name}.");
            
            var options = serviceProvider.GetService<IOptions<CosmosDBOptions>>().Value;

            if (!ValidateOptions(options)) 
            {
                Log.Fatal("Invalid CosmosDB configuration. Initialization failed.");
                throw new ConfigurationErrorsException("Invalid CosmosDB configuration during initialization.");
            }

            return Initialise<T>(options).GetAwaiter().GetResult();
        }

        private static async Task<CosmosDBStore<T>> Initialise<T>(CosmosDBOptions options) where T : Entity, IAggregateRoot
        {
            var client = new CosmosClient(options.Uri, options.Key);
            var store = new CosmosDBStore<T>(client, options);

            try
            {
                var database = await InitializeDatabase(client, options.DatabaseName);
                var containerName = EntityPluralizer.GetPlural(typeof(T).Name);
                try
                {
                    await InitializeContainer(database, containerName, "/userId");
                }
                catch (RequestFailedException)
                {
                    Log.Fatal($"Necessary CosmosDB container '{containerName}' does not exist and creation failed.");
                    throw;
                }
            }
            catch (RequestFailedException)
            {
                Log.Fatal($"Necessary CosmosDB database '{options.DatabaseName}' does not exist and creation failed.");
                throw;
            }

            return store;
        }

        private static async Task<Database> InitializeDatabase(CosmosClient client, string databaseName)
        {
            try
            {
                Log.Debug($"Verifying CosmosDB database existence for database '{databaseName}'.");
                DatabaseResponse response = await client.CreateDatabaseIfNotExistsAsync(databaseName);

                if (!response.StatusCode.IsSuccessStatusCode())
                    throw new RequestFailedException((int)response.StatusCode, "Unable to create necessary Cosmos DB database.");

                return response.Database;
            }
            catch (Exception exc)
            {
                throw new RequestFailedException("Unable to create necessary Cosmos DB database.", exc);
            }
        }

        private static async Task<Container> InitializeContainer(Database database, string containerName, string partitionKey)
        {
            try
            {
                Log.Debug($"Verifying CosmosDB container existence for container '{containerName}'.");
                ContainerResponse response = await database.CreateContainerIfNotExistsAsync(containerName, partitionKey);

                if (!response.StatusCode.IsSuccessStatusCode())
                    throw new RequestFailedException((int)response.StatusCode, "Unable to create necessary Cosmos DB container.");

                return response.Container;
            }
            catch (Exception exc)
            {
                throw new RequestFailedException("Unable to create necessary Cosmos DB container.", exc);
            }
        }

        private static bool ValidateOptions(CosmosDBOptions options) 
        {
            if (options == null)
                return false;

            if (string.IsNullOrWhiteSpace(options.DatabaseName) || string.IsNullOrWhiteSpace(options.Key) || string.IsNullOrWhiteSpace(options.Uri))
                return false;

            return true;
        }
    }
}
