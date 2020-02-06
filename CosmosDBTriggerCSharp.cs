using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Category.Function
{
    public static class CosmosDBTriggerCSharp
    {
        private static IConnectionMultiplexer _redisConnectionMultiplexer =
        ConnectionMultiplexer.Connect("chngrediscache.redis.cache.windows.net:6380,password=BrokbnFVr4uDXfNa3YF4LIdxm51fWm5mdrchwc6NpDU=,ssl=True,abortConnect=False");

        [FunctionName("CosmosDBTriggerCSharp")]
        public static void Run([CosmosDBTrigger(
            databaseName: "Contoso",
            collectionName: "Category",
            ConnectionStringSetting = "CosmosDBConnection",
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);

                var db = _redisConnectionMultiplexer.GetDatabase();

                foreach (var document in input)
                {
                    db.StringSet(document.Id, document.ToString());
                    log.LogInformation($"Saved item with id {input.Count} in Azure Redis cache");
                }

            }
        }
    }
}
