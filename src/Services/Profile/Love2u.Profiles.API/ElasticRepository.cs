using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Love2u.Profiles.API.Models;
using MongoDB.Bson.IO;

namespace Love2u.Profiles.API
{
    public class ElasticRepository
    {
        private readonly ElasticLowLevelClient _client;

        public ElasticRepository()
        {
            var host = Environment.GetEnvironmentVariable("ELASTICSEARCH_HOSTS");

            if (string.IsNullOrWhiteSpace(host)) throw new ArgumentNullException($"Invalid environment variable 'ELASTICSEARCH_HOSTS'.");

            var node = new Uri(host);
            var config = new ConnectionConfiguration(node);
            
            var client = new ElasticLowLevelClient(config);
            _client = client;
        }

        public async Task SaveProfile(UserProfile profile)
        {
            try
            {
                PostData fromObject = PostData.Serializable(profile);
                await  _client.Indices.CreateAsync<>()
                await _client.IndexAsync<StringResponse>("love2u/userprofiles", profile.Id.ToString(),
                    PostData.Serializable(new
                        {id = profile.Id, userId = profile.UserId, description = profile.Description}));

                _client.IndexAsync<>()
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
