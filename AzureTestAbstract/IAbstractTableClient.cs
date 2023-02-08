using System.Linq.Expressions;
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Azure.Data.Tables.Sas;

namespace AzureTestAbstract;

// @formatter:off
public interface IAbstractTableClient
{
    
    Pageable<T> Query<T>(Expression<Func<T, bool>> filter, int? maxPerPage = null, IEnumerable<string> select = null,CancellationToken cancellationToken = default) where T : class,  IAbstractTableEntity,   new();
    Pageable<T> Query<T>(string filter = null,int? maxPerPage = null,IEnumerable<string> select = null,CancellationToken cancellationToken = default) where T : class, IAbstractTableEntity, new();
    TableSasBuilder GetSasBuilder(TableSasPermissions permissions, DateTimeOffset expiresOn);
    TableSasBuilder GetSasBuilder(string rawPermissions, DateTimeOffset expiresOn);
    Response<IAbstractTableItem> Create(CancellationToken cancellationToken = default);
    Task<Response<IAbstractTableItem>> CreateAsync(CancellationToken cancellationToken = default);
    Response<IAbstractTableItem> CreateIfNotExists(CancellationToken cancellationToken = default);
    Task<Task<Response<IAbstractTableItem>>> CreateIfNotExistsAsync(CancellationToken cancellationToken = default);
    Response Delete(CancellationToken cancellationToken = default);
    Task<Response> DeleteAsync(CancellationToken cancellationToken = default);
    Task<Response> AddEntityAsync<T>(T entity, CancellationToken cancellationToken = default) where T : ITableEntity;
    Response AddEntity<T>(T entity, CancellationToken cancellationToken = default) where T : ITableEntity;
    Response<T> GetEntity<T>(string partitionKey, string rowKey, IEnumerable<string> select = null, CancellationToken cancellationToken = default)where T : class, ITableEntity, new();
    Task<Response<T>> GetEntityAsync<T>(string partitionKey, string rowKey, IEnumerable<string> select = null, CancellationToken cancellationToken = default);
    NullableResponse<T> GetEntityIfExists<T>(string partitionKey, string rowKey, IEnumerable<string> select = null, CancellationToken cancellationToken = default) where T : class, ITableEntity, new();
    Task<NullableResponse<T>> GetEntityIfExistsAsync<T>(string partitionKey, string rowKey,IEnumerable<string> select = null, CancellationToken cancellationToken = default)where T : class, ITableEntity, new();
    Task<Response> UpsertEntityAsync<T>(T entity,TableUpdateMode mode = TableUpdateMode.Merge,CancellationToken cancellationToken = default) where T : ITableEntity;
    Response UpsertEntity<T>(T entity, TableUpdateMode mode = TableUpdateMode.Merge,CancellationToken cancellationToken = default)where T : ITableEntity;
    Task<Response> UpdateEntityAsync<T>(T entity,ETag ifMatch,TableUpdateMode mode = TableUpdateMode.Merge,CancellationToken cancellationToken = default) where T : ITableEntity;
    Response UpdateEntity<T>(T entity, ETag ifMatch, TableUpdateMode mode = TableUpdateMode.Merge,CancellationToken cancellationToken = default)where T : ITableEntity;
    Task<Response> DeleteEntityAsync(string partitionKey,string rowKey,ETag ifMatch = default,CancellationToken cancellationToken = default);
    Response DeleteEntity(string partitionKey, string rowKey, ETag ifMatch = default,CancellationToken cancellationToken = default);
    Task<Response<IReadOnlyList<TableSignedIdentifier>>> GetAccessPoliciesAsync(CancellationToken cancellationToken = default);
    Response<IReadOnlyList<TableSignedIdentifier>> GetAccessPolicies(CancellationToken cancellationToken = default);
    Task<Response> SetAccessPolicyAsync(IEnumerable<TableSignedIdentifier> tableAcl,CancellationToken cancellationToken = default);
    Response SetAccessPolicy(IEnumerable<TableSignedIdentifier> tableAcl,CancellationToken cancellationToken = default);
    string CreateQueryFilter<T>(Expression<Func<T, bool>> filter);
    string CreateQueryFilter(FormattableString filter);
    Task<Response<IReadOnlyList<Response>>> SubmitTransactionAsync(IEnumerable<TableTransactionAction> transactionActions,CancellationToken cancellationToken = default);
    Response<IReadOnlyList<Response>> SubmitTransaction(IEnumerable<TableTransactionAction> transactionActions,CancellationToken cancellationToken = default);
    Uri GenerateSasUri(TableSasPermissions permissions, DateTimeOffset expiresOn);
    Uri GenerateSasUri(TableSasBuilder builder);}
// @formatter:on