using System.Collections;
using System.Globalization;
using Azure;
using Azure.Data.Tables;
using AzureTestAbstract.Helpers;

namespace AzureTestAbstract.Implementation;

public class AzureTableEntity : IAbstractTableEntity, ITableEntity, IDictionary<string, object>
{
    private readonly IDictionary<string, object> _properties = new Dictionary<string, object>();

    public AzureTableEntity()
    {
    }

    public AzureTableEntity(string partitionKey, string rowKey)
    {
        PartitionKey = partitionKey;
        RowKey = rowKey;
    }

    public AzureTableEntity(IDictionary<string, object> values)
    {
        _properties = values != null ? new Dictionary<string, object>(values) : new Dictionary<string, object>();
    }

    public string PartitionKey
    {
        get => GetString(PropertyNames.PartitionKey);
        set => _properties[PropertyNames.PartitionKey] = value;
    }

    public string RowKey
    {
        get => GetString(PropertyNames.RowKey);
        set => _properties[PropertyNames.RowKey] = value;
    }

    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }


    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return _properties.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_properties).GetEnumerator();
    }

    public void Add(KeyValuePair<string, object> item)
    {
        _properties.Add(item);
    }

    public void Clear()
    {
        _properties.Clear();
    }

    public bool Contains(KeyValuePair<string, object> item)
    {
        return _properties.Contains(item);
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        _properties.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, object> item)
    {
        return _properties.Remove(item);
    }

    public int Count => _properties.Count;
    public bool IsReadOnly => _properties.IsReadOnly;

    public void Add(string key, object value)
    {
        _properties.Add(key, value);
    }

    public bool ContainsKey(string key)
    {
        return _properties.ContainsKey(key);
    }

    public bool Remove(string key)
    {
        return _properties.Remove(key);
    }

    public bool TryGetValue(string key, out object value)
    {
        return _properties.TryGetValue(key, out value);
    }

    public object this[string key]
    {
        get => _properties[key];
        set => _properties[key] = value;
    }

    public ICollection<string> Keys => _properties.Keys;

    public ICollection<object> Values => _properties.Values;

    public static implicit operator AzureTableEntity(TableEntity d)
    {
        return new(d.PartitionKey, d.RowKey);
    }

    public static AzureTableEntity FromTableEntity(Response<TableEntity> r)
    {
        var d = r.Value;
        var ate = new AzureTableEntity(d.PartitionKey, d.RowKey);
        foreach (var v in d) ate[v.Key] = d[v.Key];

        return ate;
    }

    public static Task<Response<T>> FromIAbstractTableEntity<T>(Task<Response<TableEntity>> task)
        where T : IAbstractTableEntity
    {
        Response<T> r = new AzureResponse<T>();
        return Task.FromResult(r);
    }

    public static implicit operator TableEntity(AzureTableEntity d)
    {
        return new(d.PartitionKey, d.RowKey);
    }

    public string GetString(string key)
    {
        return GetValue<string>(key);
    }

    public double? GetDouble(string key)
    {
        return GetValue<double?>(key);
    }

    private T GetValue<T>(string key)
    {
        return (T)GetValue(key, typeof(T));
    }

    private object GetValue(string key, Type type = null)
    {
        // Argument.AssertNotNullOrEmpty(key, nameof(key));
        if (!_properties.TryGetValue(key, out var value) || value == null) return null;

        if (type != null)
        {
            var valueType = value.GetType();
            if (type == typeof(DateTime?) && valueType == typeof(DateTimeOffset))
                return ((DateTimeOffset)value).UtcDateTime;

            if (type == typeof(DateTimeOffset?) && valueType == typeof(DateTime))
                return new DateTimeOffset((DateTime)value);

            if (type == typeof(BinaryData) && value is byte[] byteArray) return new BinaryData(byteArray);

            EnforceType(type, valueType);
        }

        return value;
    }

    private static void EnforceType(Type requestedType, Type givenType)
    {
        if (!requestedType.IsAssignableFrom(givenType))
            throw new InvalidOperationException(string.Format(
                CultureInfo.InvariantCulture,
                $"Cannot return {requestedType} type for a {givenType} typed property."));
    }

    public static T FromIAbstractTableEntity<T>(IAbstractTableEntity entity) where T : ITableEntity
    {
        throw new NotImplementedException();
    }

    internal static class PropertyNames
    {
        public const string Timestamp = "Timestamp";
        public const string PartitionKey = "PartitionKey";
        public const string RowKey = "RowKey";
        public const string EtagOdata = "odata.etag";
        public const string ETag = "ETag";
        public const string OdataMetadata = "odata.metadata";
    }
}