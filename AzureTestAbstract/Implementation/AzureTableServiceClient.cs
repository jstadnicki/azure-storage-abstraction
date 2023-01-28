using System.Linq.Expressions;
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using AzureTestAbstract.Helpers;

namespace AzureTestAbstract.Implementation;

public class AzureTableServiceClient : IAbstractTableServiceClient
{
    private readonly TableServiceClient _wrapped;

    public AzureTableServiceClient(string storageUri, TableSharedKeyCredential tableSharedKeyCredential)
    {
        _wrapped = new TableServiceClient(
            new Uri(storageUri), tableSharedKeyCredential);
    }

    public AzureTableServiceClient(string storageUri, string tableSharedKeyCredential, string storageAccountKey) : this(
        storageUri, new TableSharedKeyCredential(tableSharedKeyCredential, storageAccountKey))
    {
    }

    public IAbstractTableItem CreateTableIfNotExists(string tableName)
    {
        return new AzureTableItem(_wrapped.CreateTableIfNotExists(tableName));
    }

    public Pageable<IAbstractTableItem> Query(string filter)
    {
        Pageable<TableItem>? x = _wrapped.Query(filter);

        var pages = x.AsPages().Select(pg =>
            Page<IAbstractTableItem>.FromValues(pg.Values.Select(pgv => new AzureTableItem(pgv)).ToList(), null, null));
        return Pageable<IAbstractTableItem>.FromPages(pages);
    }

    public Pageable<IAbstractTableItem> Query(string filter = null, int? maxPerPage = null,
        CancellationToken cancellationToken = default)
    {
        var x = _wrapped.Query(filter, maxPerPage, cancellationToken);
        var pages = x.AsPages().Select(pg =>
            Page<IAbstractTableItem>.FromValues(pg.Values.Cast<AzureTableItem>().ToList(), null, null));
        return Pageable<IAbstractTableItem>.FromPages(pages);
    }

    public Pageable<IAbstractTableItem> Query(FormattableString filter, int? maxPerPage = null,
        CancellationToken cancellationToken = default)
    {
        var x = _wrapped.Query(filter, maxPerPage, cancellationToken);

        var pages = x.AsPages().Select(pg =>
            Page<IAbstractTableItem>.FromValues(pg.Values.Cast<AzureTableItem>().ToList(), null, null));
        return Pageable<IAbstractTableItem>.FromPages(pages);
    }

    public Pageable<IAbstractTableItem> Query(Expression<Func<IAbstractTableItem, bool>> filter, int? maxPerPage = null,
        CancellationToken cancellationToken = default)
    {
        var e = MyExpressionVisitor.Convert(filter);

        var x = _wrapped.Query(e, maxPerPage, cancellationToken);

        var pages = x.AsPages().Select(pg =>
            Page<IAbstractTableItem>.FromValues(pg.Values.Cast<AzureTableItem>().ToList(), null, null));
        return Pageable<IAbstractTableItem>.FromPages(pages);
    }
}