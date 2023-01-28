using System.Linq.Expressions;
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Azure.Data.Tables.Sas;
using AzureTestAbstract.Helpers;

namespace AzureTestAbstract.Implementation;
// @formatter:off

public class AzureTableClient : IAbstractTableClient
{
    private readonly TableClient _wrapped;

    public AzureTableClient(Uri uri, string tableName, TableSharedKeyCredential tableSharedKeyCredential) =>
        _wrapped = new TableClient(uri, tableName, tableSharedKeyCredential);

    public AzureTableClient(
        string storageUri, string tableName, string tableSharedKeyCredential, string storageAccountKey) :
        this(new Uri(storageUri), tableName, new TableSharedKeyCredential(tableSharedKeyCredential, storageAccountKey))
    {
    }

    public virtual string Name 
        => _wrapped.Name;

    public virtual string AccountName 
        => _wrapped.AccountName;

    public virtual Uri Uri 
        => _wrapped.Uri;

    public void Create() 
        => _wrapped.Create();

    public void AddEntity(IAbstractTableEntity entity)
    {
        var x = AzureTableEntity.FromIAbstractTableEntity<TableEntity>(entity);
        _wrapped.AddEntity(x);
    }

    public Pageable<T> Query<T>(Expression<Func<T, bool>> filter, int? maxPerPage = null, IEnumerable<string> select = null,CancellationToken cancellationToken = default) where T : class, IAbstractTableEntity, new()
    {
        var e = MyExpressionVisitor2.Convert(filter);
        var x = _wrapped.Query(e, maxPerPage, select, cancellationToken);
        
        var pages = x.AsPages().Select(pg =>
            Page<IAbstractTableEntity>.FromValues(pg.Values.Cast<IAbstractTableEntity>().ToList(), null, null));
        
        return Pageable<T>.FromPages((IEnumerable<Page<T>>)pages);
    }

    public Pageable<T> Query<T>(string filter = null, int? maxPerPage = null, IEnumerable<string> select = null, CancellationToken cancellationToken = default) where T : IAbstractTableEntity, new()
    {
        var x = _wrapped.Query<TableEntity>(filter, maxPerPage, select, cancellationToken);
        
        var pages = x.AsPages().Select(pg =>
            Page<IAbstractTableItem>.FromValues(pg.Values.Cast<IAbstractTableItem>().ToList(), null, null));
        
        return Pageable<T>.FromPages((IEnumerable<Page<T>>)pages);
    }

    public TableSasBuilder GetSasBuilder(TableSasPermissions permissions, DateTimeOffset expiresOn) 
        => _wrapped.GetSasBuilder(permissions, expiresOn);

    public TableSasBuilder GetSasBuilder(string rawPermissions, DateTimeOffset expiresOn)
        => _wrapped.GetSasBuilder(rawPermissions, expiresOn);

    public Response<IAbstractTableItem> Create(CancellationToken cancellationToken = default) 
        => AzureTableItem.FromTableItem(_wrapped.Create(cancellationToken));

    public async Task<Task<Response<IAbstractTableItem>>> CreateAsync(CancellationToken cancellationToken = default) 
        => Task.FromResult(AzureTableItem.FromTableItem(await _wrapped.CreateAsync(cancellationToken)));

    public Response<IAbstractTableItem> CreateIfNotExists(CancellationToken cancellationToken = default) 
        => AzureTableItem.FromTableItem(_wrapped.CreateIfNotExists(cancellationToken));

    public async Task<Task<Response<IAbstractTableItem>>> CreateIfNotExistsAsync(CancellationToken cancellationToken = default) 
        => Task.FromResult(AzureTableItem.FromTableItem(await _wrapped.CreateIfNotExistsAsync(cancellationToken)));

    public Response Delete(CancellationToken cancellationToken = default)
        => _wrapped.Delete(cancellationToken);

    public Task<Response> DeleteAsync(CancellationToken cancellationToken = default) 
        => _wrapped.DeleteAsync(cancellationToken);

    public Task<Response> AddEntityAsync<T>(T entity, CancellationToken cancellationToken = default) where T : ITableEntity 
        => _wrapped.AddEntityAsync(entity, cancellationToken);

    public Response AddEntity<T>(T entity, CancellationToken cancellationToken = default) where T : ITableEntity
        => _wrapped.AddEntity(entity, cancellationToken);

    public Response<T> GetEntity<T>(string partitionKey, string rowKey, IEnumerable<string> select = null,CancellationToken cancellationToken = default) where T : class, ITableEntity, new()
        => _wrapped.GetEntity<T>(partitionKey, rowKey, select, cancellationToken);

    public async Task<Response<T>> GetEntityAsync<T>(string partitionKey, string rowKey, IEnumerable<string> select = null,CancellationToken cancellationToken = default)
    {
        var x = await _wrapped.GetEntityAsync<TableEntity>(partitionKey, rowKey, select, cancellationToken);
        AzureTableEntity y= AzureTableEntity.FromTableEntity(x);
        return new AzureResponse<T>();
    }

    public NullableResponse<T> GetEntityIfExists<T>(string partitionKey, string rowKey,IEnumerable<string> select = null,CancellationToken cancellationToken = default) where T : class, ITableEntity, new()
        => _wrapped.GetEntityIfExists<T>(partitionKey, rowKey, select, cancellationToken);

    public Task<NullableResponse<T>> GetEntityIfExistsAsync<T>(string partitionKey, string rowKey, IEnumerable<string> select = null, CancellationToken cancellationToken = default) where T : class, ITableEntity, new()
        => _wrapped.GetEntityIfExistsAsync<T>(partitionKey, rowKey, select, cancellationToken);
    
    public Task<Response> UpsertEntityAsync<T>(T entity, TableUpdateMode mode = TableUpdateMode.Merge,CancellationToken cancellationToken = default) where T : ITableEntity
        => _wrapped.UpsertEntityAsync<T>(entity, mode, cancellationToken);

    public Response UpsertEntity<T>(T entity, TableUpdateMode mode = TableUpdateMode.Merge,CancellationToken cancellationToken = default) where T : ITableEntity
        => _wrapped.UpsertEntity<T>(entity, mode, cancellationToken);
    
    public Task<Response> UpdateEntityAsync<T>(T entity, ETag ifMatch, TableUpdateMode mode = TableUpdateMode.Merge,CancellationToken cancellationToken = default) where T : ITableEntity
        => _wrapped.UpdateEntityAsync<T>(entity, ifMatch, mode, cancellationToken);
    
    public Response UpdateEntity<T>(T entity, ETag ifMatch, TableUpdateMode mode = TableUpdateMode.Merge,CancellationToken cancellationToken = default) where T : ITableEntity
        => _wrapped.UpdateEntity(entity, ifMatch, mode, cancellationToken);

    public Task<Response> DeleteEntityAsync(string partitionKey, string rowKey, ETag ifMatch = default,CancellationToken cancellationToken = default)
        => _wrapped.DeleteEntityAsync(partitionKey, rowKey, ifMatch, cancellationToken);

    public Response DeleteEntity(string partitionKey, string rowKey, ETag ifMatch = default,CancellationToken cancellationToken = default)
        => _wrapped.DeleteEntity(partitionKey, rowKey, ifMatch, cancellationToken);

    public Task<Response<IReadOnlyList<TableSignedIdentifier>>> GetAccessPoliciesAsync(CancellationToken cancellationToken = default)
        => _wrapped.GetAccessPoliciesAsync(cancellationToken);

    public Response<IReadOnlyList<TableSignedIdentifier>> GetAccessPolicies(CancellationToken cancellationToken = default)
        => _wrapped.GetAccessPolicies(cancellationToken);

    public Task<Response> SetAccessPolicyAsync(IEnumerable<TableSignedIdentifier> tableAcl,CancellationToken cancellationToken = default)
        => _wrapped.SetAccessPolicyAsync(tableAcl, cancellationToken);

    public Response SetAccessPolicy(IEnumerable<TableSignedIdentifier> tableAcl,CancellationToken cancellationToken = default)
        => _wrapped.SetAccessPolicy(tableAcl, cancellationToken);

    public string CreateQueryFilter<T>(Expression<Func<T, bool>> filter) 
        => TableClient.CreateQueryFilter(filter);

    public string CreateQueryFilter(FormattableString filter)
        => TableClient.CreateQueryFilter(filter);

    public Task<Response<IReadOnlyList<Response>>> SubmitTransactionAsync(IEnumerable<TableTransactionAction> transactionActions, CancellationToken cancellationToken = default)
        => _wrapped.SubmitTransactionAsync(transactionActions, cancellationToken);

    public Response<IReadOnlyList<Response>> SubmitTransaction(IEnumerable<TableTransactionAction> transactionActions,CancellationToken cancellationToken = default)
        => _wrapped.SubmitTransaction(transactionActions, cancellationToken);

    public Uri GenerateSasUri(TableSasPermissions permissions, DateTimeOffset expiresOn)
        => _wrapped.GenerateSasUri(permissions, expiresOn);

    public Uri GenerateSasUri(TableSasBuilder builder) 
        => _wrapped.GenerateSasUri(builder);
}
// @formatter:on