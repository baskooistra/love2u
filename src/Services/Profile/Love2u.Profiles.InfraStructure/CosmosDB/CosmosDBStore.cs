using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Love2u.Profiles.Domain.Models;
using Love2u.Profiles.Domain.Models.Shared;
using Love2u.Profiles.Domain.Services;
using Love2u.Profiles.InfraStructure.CosmosDB.UserProfiles;
using Love2u.Profiles.InfraStructure.Extensions;
using Microsoft.Azure.Cosmos;
using Serilog;

namespace Love2u.Profiles.InfraStructure.CosmosDB
{
    internal class CosmosDBStore<T> : IDataStore<T> where T : Entity, IAggregateRoot
    {
        private readonly Container _container;

        public CosmosDBStore(CosmosClient client, CosmosDBOptions options)
        {
            // Get the type name and pluralize it
            var containerName = EntityPluralizer.GetPlural(typeof(T).Name);

            _container = client.GetContainer(options.DatabaseName, containerName);
        }

        async Task<DataStoreResult<T>> IDataStore<T>.GetItem(Guid id, CancellationToken cancellationToken) 
        {
            try
            {
                Log.Information($"Retieving item with Id '{id}'...");

                ItemResponse<T> result = await _container.ReadItemAsync<T>(id.ToString(), new PartitionKey(id.ToString()), cancellationToken: cancellationToken);
                if (result.StatusCode.IsSuccessStatusCode())
                {
                    Log.Information("Succesfully retrieved item.", result.Resource);
                    return new DataStoreResult<T>(result.Resource, ResultType.Ok, result.ETag);
                }
                else
                {
                    Log.Information($"CosmosDB response status code of {result.StatusCode} does not indicate success.");
                    throw new Exception("Something went wrong while retrieving the document from CosmosDB.");
                }
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Log.Information($"Unable top find item with Id '{id}'.");
                return new DataStoreResult<T>(null, ResultType.Notfound, null);
            }
            catch (CosmosException exc)
            {
                Log.Error(exc, $"Error while retrieving item with id '{id}'.");
                throw;
            }
        }

        async Task<DataStoreResult<T>> IDataStore<T>.AddItem(T item, CancellationToken cancellationToken)
        {
            var partitionKeyValue = item.GetStringPartitionKeyValue();

            try
            {
                Log.Information("Persisting item...", item);
                var result = await _container.CreateItemAsync<T>(item, new PartitionKey(partitionKeyValue));

                if (result.StatusCode.IsSuccessStatusCode()) 
                {
                    Log.Information("Succesfully persisted item.", item);
                    return new DataStoreResult<T>(item, ResultType.Created, result.ETag);
                }
                else
                {
                    Log.Information($"CosmosDB response status code of {result.StatusCode} does not indicate success.");
                    throw new Exception("Something went wrong while persisting the document to CosmosDB.");
                }
            }
            catch (CosmosException exc)
            {
                Log.Error(exc, "Error while persisting item.", item);
                throw;
            }
        }

        async Task<DataStoreResult<bool>> IDataStore<T>.DeleteItem(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                Log.Information($"Deleting item...", id);
                var result = await _container.DeleteItemAsync<T>(id.ToString(), new PartitionKey(id.ToString()), cancellationToken: cancellationToken);

                if (result.StatusCode.IsSuccessStatusCode())
                {
                    Log.Information("Succesfully deleted item.", id);
                    return new DataStoreResult<bool>(true, ResultType.Ok, null);
                }
                else
                {
                    Log.Information($"CosmosDB response status code of {result.StatusCode} does not indicate success.");
                    throw new Exception("Something went wrong while deleting the document to CosmosDB.");
                }
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Log.Information($"Unable to find item with Id '{id}'.");
                return new DataStoreResult<bool>(false, ResultType.Notfound, null);
            }
            catch (CosmosException exc)
            {
                Log.Error(exc, $"Error while deleting item with id '{id}'.");
                throw;
            }
        }

        async Task<DataStoreResult<T>> IDataStore<T>.UpdateItem(T item, CancellationToken cancellationToken)
        {
            var partitionKeyValue = item.GetStringPartitionKeyValue();

            try
            {
                Log.Information("Persisting item...", item);
                var result = await _container.ReplaceItemAsync(item, item.Id.ToString(), new PartitionKey(partitionKeyValue), cancellationToken: cancellationToken);

                if (result.StatusCode.IsSuccessStatusCode())
                {
                    Log.Information("Succesfully updated item.", item);
                    return new DataStoreResult<T>(item, ResultType.Updated, result.ETag);
                }
                else if (result.StatusCode == HttpStatusCode.NotFound) 
                {
                    Log.Information($"Unable to find item with Id '{item.Id}'.");
                    return new DataStoreResult<T>(item, ResultType.Notfound, null);
                }
                else
                {
                    Log.Information($"CosmosDB response status code of {result.StatusCode} does not indicate success.");
                    throw new Exception("Something went wrong while updating the document to CosmosDB.");
                }
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Log.Information($"Unable to find item with Id '{item.Id}'.");
                return new DataStoreResult<T>(item, ResultType.Notfound, null);
            }
            catch (CosmosException exc)
            {
                Log.Error(exc, $"Error while updating item with id '{item.Id}'.");
                throw;
            }
        }
    }
}