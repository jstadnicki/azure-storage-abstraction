using Azure;
using AzureTestAbstract.Implementation;

namespace AzureTestAbstract;

public class TableConsumer
{
    private IAbstractTableClient _abstractTableClient;
    private IAbstractTableServiceClient _serviceClient;

    public TableConsumer(IAbstractTableClient abstractTableClient, IAbstractTableServiceClient serviceClient)
    {
        _abstractTableClient = abstractTableClient;
        _serviceClient = serviceClient;
    }

    public void WorkTest()
    {
        // string storageUri = "";
        // string accountName = "";

        // string storageAccountKey = "";

        string tableName = "OfficeSupplies1p1";
        var table = _serviceClient.CreateTableIfNotExists(tableName);
        Console.WriteLine($"The created table's name is {table.Name}.");

        Pageable<IAbstractTableItem> queryTableResults = _serviceClient.Query(filter: $"TableName eq '{tableName}'");

        Console.WriteLine("The following are the names of the tables in the query results:");

        foreach (IAbstractTableItem t in queryTableResults)
        {
            Console.WriteLine(t.Name);
        }

        // _abstractTableClient = new AzureTableClient(storageUri, tableName, accountName, storageAccountKey);

        // _abstractTableClient.Create();

        string partitionKey = Guid.NewGuid().ToString();
        string rowKey = Guid.NewGuid().ToString();
        var entity = new AzureTableEntity(partitionKey, rowKey)
        {
            { "Product", "Marker Set" },
            { "Price", 5.00 },
            { "Quantity", 21 }
        };

        _abstractTableClient.AddEntity(entity);

        Pageable<AzureTableEntity> queryResultsFilter =
            _abstractTableClient.Query<AzureTableEntity>(filter: $"PartitionKey eq '{partitionKey}'");

        foreach (AzureTableEntity qEntity in queryResultsFilter)
        {
            Console.WriteLine($"{qEntity.GetString("Product")}: {qEntity.GetDouble("Price")}");
        }

        Console.WriteLine($"The query returned {queryResultsFilter.Count()} entities.");
    }
}