using Love2u.Profiles.Domain.Models;
using Love2u.Profiles.Domain.Models.Aggregates.UserProfile;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Love2u.Profiles.InfraStructure.CosmosDB.UserProfiles
{
    internal static class EntityExtensions
    {
        internal static string GetStringPartitionKeyValue<T>(this T entity) where T : Entity, IAggregateRoot
        {
            switch (entity) 
            {
                case UserProfile profile:
                    return profile.UserId.ToString();
                default:
                    Log.Error($"Unable to retrieve CosmosDB partition key. Unknown type {typeof(T).Name}.");
                    throw new InvalidOperationException($"Invalid item type '{typeof(T).Name}' passed when fetching CosmosDB partition key value.");
            }
        }
    }
}
