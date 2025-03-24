using Microsoft.Azure.Cosmos;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosFailoverApp
{
    public static class RetryPolicyFactory
    {
        public static AsyncRetryPolicy CreateRetryPolicy(ConsoleLogger logger)
        {
            return Policy
                .Handle<CosmosException>(ex => (int)ex.StatusCode == 429 || (int)ex.StatusCode >= 500)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        logger.Error($"Retry {retryCount} due to {exception.Message}");
                    });
        }
    }
}
