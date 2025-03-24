using Microsoft.Azure.Cosmos;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosFailoverApp
{
    public class FailOverCosmosDbClient
    {
        private readonly CosmosClient[] clients;
        private readonly ConsoleLogger logger;
        private readonly AsyncRetryPolicy retryPolicy;
        private int currentIndex = 0;

        public FailOverCosmosDbClient(CosmosClient c1, CosmosClient c2, ConsoleLogger logger, AsyncRetryPolicy retryPolicy)
        {
            clients = new[] { c1, c2 };
            this.logger = logger;
            this.retryPolicy = retryPolicy;
        }

        private CosmosClient Current => clients[currentIndex];
        private CosmosClient Fallback => clients[1 - currentIndex];

        public async Task WriteAsync<T>(string docId, string db, string container, string vehicleId, T item, string partitionKey)
        {
            try
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    var cont = Current.GetContainer(db, container);
                    var response = await cont.UpsertItemAsync(item, new PartitionKey(partitionKey));
                    logger.Info($"[Write] RU: {response.RequestCharge} PKRange: {response.Headers["x-ms-documentdb-partitionkeyrangeid"]}");
                });
            }
            catch (CosmosException ex)
            {
                logger.Error($"Primary failed: {ex.Message}. Retrying on fallback...");
                currentIndex = 1 - currentIndex; // Switch to fallback
                await retryPolicy.ExecuteAsync(async () =>
                {
                    var cont = Current.GetContainer(db, container);
                    var response = await cont.UpsertItemAsync(item, new PartitionKey(partitionKey));
                    logger.Info($"[Write-Fallback] RU: {response.RequestCharge} PKRange: {response.Headers["x-ms-documentdb-partitionkeyrangeid"]}");
                });
            }
        }

        public async Task<T> ReadAsync<T>(string docId, string db, string container, string vehicleId, string partitionKey)
        {
            try
            {
                return await retryPolicy.ExecuteAsync(async () =>
                {
                    var cont = Current.GetContainer(db, container);
                    var response = await cont.ReadItemAsync<T>(docId, new PartitionKey(partitionKey));
                    logger.Info($"[Read] RU: {response.RequestCharge} PKRange: {response.Headers["x-ms-documentdb-partitionkeyrangeid"]}");
                    return response.Resource;
                });
            }
            catch (CosmosException ex)
            {
                logger.Error($"Primary read failed: {ex.Message}. Retrying on fallback...");
                currentIndex = 1 - currentIndex;
                return await retryPolicy.ExecuteAsync(async () =>
                {
                    var cont = Current.GetContainer(db, container);
                    var response = await cont.ReadItemAsync<T>(docId, new PartitionKey(partitionKey));
                    logger.Info($"[Read-Fallback] RU: {response.RequestCharge} PKRange: {response.Headers["x-ms-documentdb-partitionkeyrangeid"]}");
                    return response.Resource;
                });
            }
        }
    }
}
