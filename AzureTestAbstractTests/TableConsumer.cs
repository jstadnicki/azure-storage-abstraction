using System.Runtime.CompilerServices;
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Azure.Data.Tables.Sas;
using AzureTestAbstract.Implementation;
using Newtonsoft.Json;

namespace AzureTestAbstract;

public class TableConsumer
{
    private readonly IAbstractTableClient _abstractTableClient;
    private readonly IAbstractTableServiceClient _serviceClient;

    public TableConsumer(IAbstractTableClient abstractTableClient, IAbstractTableServiceClient serviceClient)
    {
        _abstractTableClient = abstractTableClient;
        _serviceClient = serviceClient;
    }

    public Response AddEntity(string pkey, string rkey)
    {
        var entity = new AzureTableEntity(pkey, rkey);
        entity["string"] = "string";
        entity["int"] = "1";
        entity["decimal"] = 1m.ToString();
        entity["double"] = 1.0d.ToString();
        entity["object"] = new object().ToString();
        entity["json"] = JsonConvert.SerializeObject(new List<TableConsumerTestClass>
            { new(), new() });

        return _abstractTableClient.AddEntity(entity);
    }

    public async Task<Response> AddEntityAsync(string pkey, string rkey)
    {
        var entity = new AzureTableEntity(pkey, rkey);
        entity["string"] = "string";
        entity["int"] = "1";
        entity["decimal"] = 1m.ToString();
        entity["double"] = 1.0d.ToString();
        entity["object"] = new object().ToString();
        entity["json"] = JsonConvert.SerializeObject(new List<TableConsumerTestClass> { new(), new() });

        return await _abstractTableClient.AddEntityAsync(entity);
    }

    public Pageable<AzureTableEntity> Query() => _abstractTableClient.Query<AzureTableEntity>(x1 => x1.PartitionKey == "123");

    public Pageable<AzureTableEntity> QueryWithString() => _abstractTableClient.Query<AzureTableEntity>("partitionkey eq 123", 15);

    public Response<IAbstractTableItem> T_Create() => _abstractTableClient.Create();

    public Response T_Delete() => _abstractTableClient.Delete();

    public async Task<Response<IAbstractTableItem>> T_CreateAsync() => await _abstractTableClient.CreateAsync();

    public async Task<Response> T_DeleteAsync() => await _abstractTableClient.DeleteAsync();

    public AzureTableEntity T_GetEntity(string pKey, string rKey)
    {
        return _abstractTableClient.GetEntity<AzureTableEntity>(pKey, rKey);
    }

    public Response<IAbstractTableItem> T_CreateIfNotExists()
    {
        return _abstractTableClient.CreateIfNotExists();
    }

    public async Task<Task<Response<IAbstractTableItem>>> T_CreateIfNotExistsAsync()
    {
        return await _abstractTableClient.CreateIfNotExistsAsync();
    }

    public TableSasBuilder T_GetSasBuilder() => _abstractTableClient.GetSasBuilder(TableSasPermissions.Add, DateTimeOffset.Now);

    public TableSasBuilder T_GetSasBuilderWithString() => _abstractTableClient.GetSasBuilder("asad", DateTimeOffset.Now);

    public async Task<Response<AzureTableEntity>> T_GetEntityAsync(string pKey, string rKey)
    {
        return await _abstractTableClient.GetEntityAsync<AzureTableEntity>(pKey, rKey);
    }

    public NullableResponse<AzureTableEntity> T_GetEntityIfExists(string pKey, string rKey)
    {
        return _abstractTableClient.GetEntityIfExists<AzureTableEntity>(pKey, rKey);
    }

    public async Task<NullableResponse<AzureTableEntity>> T_GetEntityIfExistsAsync(string pKey, string rKey)
    {
        return await _abstractTableClient.GetEntityIfExistsAsync<AzureTableEntity>(pKey, rKey);
    }

    public Response<IReadOnlyList<Response>> T_SubmitTransaction()
    {
        var t = new List<TableTransactionAction> { new TableTransactionAction(TableTransactionActionType.Add, new TableEntity()) };
        return _abstractTableClient.SubmitTransaction(t);
    }

    public async Task<Response<IReadOnlyList<Response>>> T_SubmitTransactionAsync()
    {
        var t = new List<TableTransactionAction> { new TableTransactionAction(TableTransactionActionType.Add, new TableEntity()) };
        return await _abstractTableClient.SubmitTransactionAsync(t);
    }

    public Response T_UpsertEntity()
    {
        var e = new AzureTableEntity();
        return _abstractTableClient.UpsertEntity(e);
    }

    public async Task<Response> T_UpsertEntityAsync()
    {
        var e = new AzureTableEntity();
        return await _abstractTableClient.UpsertEntityAsync(e);
    }

    public Response T_UpdateEntity()
    {
        var e = new AzureTableEntity();
        return _abstractTableClient.UpdateEntity(e, new ETag("asd"));
    }

    public async Task<Response> T_UpdateEntityAsync()
    {
        var e = new AzureTableEntity();
        return await _abstractTableClient.UpdateEntityAsync(e, new ETag("asd"));
    }

    public Response T_Delete(string partitionKey, string rowKey)
    {
        return _abstractTableClient.DeleteEntity(partitionKey, rowKey);
    }

    public async Task<Response> T_DeleteAsync(string partitionKey, string rowKey)
    {
        return await _abstractTableClient.DeleteEntityAsync(partitionKey, rowKey);
    }

    public Response<IReadOnlyList<TableSignedIdentifier>> T_GetAccessPolicies()
    {
        return _abstractTableClient.GetAccessPolicies();
    }

    public async Task<Response<IReadOnlyList<TableSignedIdentifier>>> T_GetAccessPoliciesAsync()
    {
        return await _abstractTableClient.GetAccessPoliciesAsync();
    }

    public Response T_SetAccessPolicy()
    {
        return _abstractTableClient.SetAccessPolicy(new List<TableSignedIdentifier>());
    }

    public async Task<Response> T_SetAccessPolicyAsync()
    {
        return await _abstractTableClient.SetAccessPolicyAsync(new List<TableSignedIdentifier>());
    }

    public string T_CreateQueryFilter()
    {
        return _abstractTableClient.CreateQueryFilter<AzureTableEntity>(x => x.PartitionKey == "123");
    }
    
    public string T_CreateQueryFilterWithString()
    {
        FormattableString x = FormattableStringFactory.Create("{0}", 0); 
        return _abstractTableClient.CreateQueryFilter(x);
    }

    public Uri T_GenerateSasUri()
    {
        return _abstractTableClient.GenerateSasUri(TableSasPermissions.Add, new DateTimeOffset(DateTime.Now));
    }

    public Uri T_GenerateSasUriWithSasBuilder()
    {
        TableSasBuilder b = new TableSasBuilder("asdoiasd", "asdasd", DateTimeOffset.Now);
        return _abstractTableClient.GenerateSasUri(b);
    }


    public void WorkTest()
    {
        var tableName = "OfficeSupplies1p1";
        var table = _serviceClient.CreateTableIfNotExists(tableName);
        Console.WriteLine($"The created table's name is {table.Name}.");

        var queryTableResults = _serviceClient.Query($"TableName eq '{tableName}'");

        Console.WriteLine("The following are the names of the tables in the query results:");

        foreach (var t in queryTableResults) Console.WriteLine(t.Name);

        // _abstractTableClient = new AzureTableClient(storageUri, tableName, accountName, storageAccountKey);

        // _abstractTableClient.Create();

        var partitionKey = Guid.NewGuid().ToString();
        var rowKey = Guid.NewGuid().ToString();
        var entity = new AzureTableEntity(partitionKey, rowKey)
        {
            { "Product", "Marker Set" },
            { "Price", 5.00 },
            { "Quantity", 21 }
        };

        _abstractTableClient.AddEntity(entity);

        var queryResultsFilter =
            _abstractTableClient.Query<AzureTableEntity>($"PartitionKey eq '{partitionKey}'");

        foreach (var qEntity in queryResultsFilter)
            Console.WriteLine($"{qEntity.GetString("Product")}: {qEntity.GetDouble("Price")}");

        Console.WriteLine($"The query returned {queryResultsFilter.Count()} entities.");
    }

    public class TableConsumerTestClass
    {
        public string Text { get; set; } = "test";
        public int Value { get; set; } = 1;
    }
}