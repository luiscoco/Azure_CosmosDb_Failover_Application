#   Azure CosmosDb Failover Application

This app shows:

CosmosDB CreateItem + ReadItem

Retry with Polly

Failover between two clients

Console logging of RU charges and PartitionKeyRangeId

## 1. Prerequisites

Create an Azure Cosmos DB for NoSQL account.

Create:

Database: SampleDb

Container: Users

Partition Key: /userId

Save your connection string from the portal.

## 2. Create the Azure CosmosDb database and container

### 2.1. Sign in to Azure

Go to 👉 https://portal.azure.com

### 2.2. Create Cosmos DB Account

Search for Cosmos DB in the top search bar.

Click "Create".

Choose "Azure Cosmos DB for NoSQL" as the API.

Fill in the required info:

Subscription: Select yours

Resource Group: Create one or select existing (e.g., CosmosDemoRG)

Account Name: e.g., failovercosmosdemo

Region: Choose closest to you

Leave other defaults

Click "Review + Create", then "Create"

Done: This creates your Cosmos DB account

### 2.3. Create Database and Container

Once the account is deployed:

Open your Cosmos DB resource (from the deployment page or search).

In the left-hand menu, click "Data Explorer"

Click "New Container"

Fill out the form:

Database id: SampleDb (create new)

Container id: Users

Partition key: /userId

Throughput: Choose Manual or Autoscale (default 400 RU/s is fine)

Click OK

 one: You now have the SampleDb database with a Users container partitioned on /userId

### 2.4. Get Your Connection String

In the left menu, go to "Keys"

Copy:

URI

Primary Key

Or scroll down to find the full Connection String

You’ll use this in your C# app like this:

```csharp
private const string ConnectionString = "AccountEndpoint=https://your-account.documents.azure.com:443/;AccountKey=your-key;";
```

### 2.5. Test Your App

You’re now ready to run your console app locally:

Paste the connection string into Program.cs

Run:

```
dotnet run
```
