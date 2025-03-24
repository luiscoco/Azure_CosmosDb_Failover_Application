#   Azure CosmosDb Failover Application

This app shows:

CosmosDB CreateItem + ReadItem

Retry with Polly

Failover between two clients

Console logging of RU charges and PartitionKeyRangeId

## Prerequisites

Create an Azure Cosmos DB for NoSQL account.

Create:

Database: SampleDb

Container: Users

Partition Key: /userId

Save your connection string from the portal.


