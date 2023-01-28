using Azure;

namespace AzureTestAbstract;

public interface IAbstractTableEntity : IDictionary<string, object>
{
    string PartitionKey { get; set; }
    string RowKey { get; set; }
    DateTimeOffset? Timestamp { get; set; }
    ETag ETag { get; set; }
}