using System.Linq.Expressions;
using Azure;

namespace AzureTestAbstract;

public interface IAbstractTableServiceClient
{
    IAbstractTableItem CreateTableIfNotExists(string tableName);
    Pageable<IAbstractTableItem> Query(string filter);

    Pageable<IAbstractTableItem> Query(string filter = null, int? maxPerPage = null,
        CancellationToken cancellationToken = default);

    Pageable<IAbstractTableItem> Query(FormattableString filter, int? maxPerPage = null,
        CancellationToken cancellationToken = default);

    Pageable<IAbstractTableItem> Query(Expression<Func<IAbstractTableItem, bool>> filter, int? maxPerPage = null,
        CancellationToken cancellationToken = default);
}