Title: "Use Pulumi to Specify Cosmos DB Indexes"
Published: 2023/05/16
Tags: 
- Blog
- Cosmos DB
- Pulumi
---

## TL;DR
Reducing the indexes Cosmos DB has to generate from your documents can reduce how much you are charged.

## So Many Indexes

Cosmos DB is a database. Which means it indexes stuff, because indexes are important.

By default, Cosmos DB indexes each field in every uploaded document. This *can* be good, if you need to be able to query each field. But more often than not, it's a waste of resources because it needs to spend more time maintaining the indexes when a document is created/updated/deleted. This translates to more time for those operations, and also increases the amount of Request Units you are charged. Meaning the default option costs you more money.

Through the Azure Portal you can manually set which fields are indexed (boo!), or you can automate it (woo!).

## What are Cosmos DB Request Units?

Request Units are an abstraction to how you are charged for using Cosmos DB. Instead of saying you want X CPU power and Y RAM, you get the same amount of performance as everyone else, but you pay for the usage. For each query to make against Cosmos DB, the HTTP Response will contain some headers about the query. One of those headers tells you how many Request Units you were charged for the query. A simple insert will be less than 10 (assuming it's a small document). A query can be 10s to 100s to 1,000s of Request Units depending on how much work the query does. 

That's the basic idea. If you want to know more, read the documentation from Microsoft: https://learn.microsoft.com/en-us/azure/cosmos-db/request-units 

## How Much Can Removing Some Indexes Matter?

Imagine a document like this:
```json
{
    "id": "8bc543ba-c765-42d3-acfb-6289d830d3c0",
    "FirstName": "Homer",
    "MiddleName": "Jay",
    "LastName": "Simpson",
    "Age": 39,
    "Address": {
        "Street": "742 Evergreen Terrace",
        "City": "Springfield",
        "State": "Unknown"
    }
}
```

This example has 9 fields, including 1 child object. 

If we know for a fact that our application will only ever query for the `id`, `FirstName`, and `LastName` fields, we can tell Cosmos DB to only index those. Below is a table comparing costs when uploading this document with all fields indexed vs just 3 fields indexed.

<table style="border: 1px solid #ddd;">
    <col style="width:33.33%;border-right: 1px solid #ddd;" />
    <col style="width:33.33%;border-right: 1px solid #ddd;" />
    <col style="width:33.33%;" />
    <thead>
        <tr style="border-bottom: 1px solid #ddd;">
            <td>Type</td>
            <td>Execution Time</td>
            <td>Request Units</td>
        </tr>
    </thead>
    <tbody>
        <tr style="border-bottom: 1px solid #ddd;">
            <td>All Indexed</td>
            <td>27.946 ms</td>
            <td>8.57</td>
        </tr>
        <tr>
            <td>3 Fields Indexed</td>
            <td>19.718 ms</td>
            <td>6.29</td>
        </tr>
    </tbody>
</table>

You can see that when there are less fields indexed, you are charged less Request Units. Does it really affect your cloud bill? Depends on your scale. But hey, why not optimize for this while you can? It probably won't hurt.

For fun, let's try this again with a larger document. Imagine this:
```json
{
    "id": "8bc543ba-c765-42d3-acfb-6289d830d3c0",
    "FirstName": "Homer",
    "MiddleName": "Jay",
    "LastName": "Simpson",
    "Age": 40,
    "Address": {
        "Street": "742 Evergreen Terrace",
        "City": "Springfield",
        "State": "Unknown"
    },
    "Occupation": "Safety Inspector",
    "Credit Score": 120,
    "Spouse": {
        "FirstName": "Marjorie",
        "MiddleName": "Jacqueline",
        "LastName": "Simpson",
        "Age": 40
    },
    "Children":[
        {
            "FirstName": "Bartholomew",
            "MiddleName": "Jojo",
            "LastName": "Simpson",
            "Age": 12
        },
        {
            "FirstName": "Lisa",
            "MiddleName": "Marie",
            "LastName": "Simpson",
            "Age": 8
        },
        {
            "FirstName": "Margaret",
            "MiddleName": "Evelyn Lenny",
            "LastName": "Simpson",
            "Age": 2
        }
    ]
}
```

This example has 27 fields, including 3 child objects, one of which is an array of 3 more child objects. 

If we know for a fact that our application will only ever query for the `id`, `FirstName`, and `LastName` fields (like the above sample), we can tell Cosmos DB to only index those. Below is a table comparing costs when uploading this document with all fields indexed vs just 3 fields indexed.

<table style="border: 1px solid #ddd;">
    <col style="width:33.33%;border-right: 1px solid #ddd;" />
    <col style="width:33.33%;border-right: 1px solid #ddd;" />
    <col style="width:33.33%;" />
    <thead>
        <tr style="border-bottom: 1px solid #ddd;">
            <td>Type</td>
            <td>Execution Time</td>
            <td>Request Units</td>
        </tr>
    </thead>
    <tbody>
        <tr style="border-bottom: 1px solid #ddd;">
            <td>All Indexed</td>
            <td>21.553 ms</td>
            <td>15.43</td>
        </tr>
        <tr>
            <td>3 Fields Indexed</td>
            <td>28.143 ms</td>
            <td>6.29</td>
        </tr>
    </tbody>
</table>

In this case the difference between Request Units cost is larger than what we saw with the smaller document. And another thing to note is that the Request Units cost is the same for the small and large documents when only indexing the 3 fields.

The execution time in this example is slightly longer when only indexing 3 fields instead of all of them. I assume the difference is luck of the draw based on load in Azure. There could be more to it, but I haven't done any research.

If your application uses documents that are usually larger or smaller, I encourage you to do your own testing to see how much you gain from making this type of change.

For refrence, here is the JSON used to specify we only want to include the `FirstName` and `LastName` fields. Note that the `id` field is automatically indexed because that is the default Partition key.
```json
{
    "indexingMode": "consistent",
    "automatic": true,
    "includedPaths": [
        {
            "path": "/FirstName/?"
        },
        {
            "path": "/LastName/?"
        }
    ],
    "excludedPaths": [
        {
            "path": "/*"
        }
    ]
}
```

## The (Automated) Solution

When possible I use Pulumi to deploy everything, the cloud infrastructure and the app itself. So my strategy to automate which fields are indexed is to:

1. During development time, use a custom attribute to mark the fields that should be indexed
2. When deploying with Pulumi, scan the binary for database entities and their indexes
3. When creating the Cosmos DB with Pulumi, create an indexing policy for each of the fields

## How to Use the Solution

To do this with C#, I created some custom NuGet packages. The code for those are hosted on GitHub at: https://github.com/Purple-Spike/cosmos-db-index-configurator

There are 3 steps:

1. Mark up your application to specify which fields to index
  - In your application, add a reference to the `PurpleSpikeProductions.CosmosDbIndexConfigurator.ConfigurationLib` NuGet package
  - For each of your database entity classes, add the `IdConfiguredEntityAttribute` attribute to the class
    - Assuming you use a custom field for the Partition Key, include the `IncludePartitionKeyAttribute` attribute over the field that is used as the Partition Key
    - For each field that you want to index, including the Partition Key field, add the `IncludeIndexAttribute` attribute

2. With Pulumi, scan the application assembly for which fields to index
  - In your Pulumi application, add a reference to the `PurpleSpikeProductions.CosmosDbIndexConfigurator.IndexMapper` NuGet package
  - Scan the assembly by using something like:
```csharp
var assemblyPath = $"PATH_TO_YOUR_APP_ASSEMBLY_FILE_HERE";
var myAssembly = Assembly.LoadFrom(assemblyPath);

var mapper = new CosmosDbIndexMapper();
DbMappedIndexes = mapper.MapIndexes(myAssembly);
```

3. With Pulumi, create a new Cosmos DB Container for each entity
```csharp
using AzureNative = Pulumi.AzureNative;

foreach (var mappedIndex in DbMappedIndexes)
{
    _ = new AzureNative.DocumentDB.SqlResourceSqlContainer(mappedIndex.ContainerName, new AzureNative.DocumentDB.SqlResourceSqlContainerArgs
    {
        ResourceGroupName = "MyResourceGroupName",
        AccountName = "MyCosmosdbAccountName",
        DatabaseName = "MyCosmosDbName",
        Resource = new AzureNative.DocumentDB.Inputs.SqlContainerResourceArgs
        {
            Id = mappedIndex.ContainerName,
            PartitionKey = new AzureNative.DocumentDB.Inputs.ContainerPartitionKeyArgs
            {
                Paths = { mappedIndex.PartitionKey! },
                Kind = "Hash"
            },
            IndexingPolicy = new AzureNative.DocumentDB.Inputs.IndexingPolicyArgs
            {
                //Default to Exclude everything from indexing
                ExcludedPaths = new AzureNative.DocumentDB.Inputs.ExcludedPathArgs
                {
                    Path = "/*"
                },
                // Include ONLY the specific fields that have the include attribute
                IncludedPaths = mappedIndex.IncludedIndexes.Select(indexPath => new AzureNative.DocumentDB.Inputs.IncludedPathArgs
                {
                    Path = indexPath,
                }).ToArray(),
            }
        },
    });
}
```
