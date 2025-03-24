using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;

namespace CosmosFailoverApp
{
    class Program
    {
        private const string ConnectionString = "<YOUR-COSMOS-CONNECTION-STRING>";
        private const string DatabaseName = "SampleDb";
        private const string ContainerName = "Users";

        static async Task Main()
        {
            var logger = new ConsoleLogger();
            var retryPolicy = RetryPolicyFactory.CreateRetryPolicy(logger);

            CosmosClient client1 = new(ConnectionString);
            CosmosClient client2 = new(ConnectionString); // Simulating secondary

            var failoverClient = new FailOverCosmosDbClient(client1, client2, logger, retryPolicy);

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "user123",
                Name = "Alice"
            };

            Console.WriteLine("Writing document...");
            await failoverClient.WriteAsync(user.Id, DatabaseName, ContainerName, user.UserId, user, user.UserId);

            Console.WriteLine("Reading document...");
            var read = await failoverClient.ReadAsync<User>(user.Id, DatabaseName, ContainerName, user.UserId, user.UserId);
            Console.WriteLine($"Read: {read?.Name}");
        }
    }
}

