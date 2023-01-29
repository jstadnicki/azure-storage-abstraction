using Azure;
using Azure.Data.Tables.Models;
using AzureTestAbstract.Helpers;

namespace AzureTestAbstract.Implementation;

public class AzureTableItem : IAbstractTableItem
{
    private readonly TableItem _wrapped;

    public AzureTableItem(Response<TableItem> createTableIfNotExists)
    {
        _wrapped = createTableIfNotExists.Value;
    }

    public AzureTableItem(TableItem tableItem)
    {
        _wrapped = tableItem;
    }

    public AzureTableItem()
    {
    }

    public string Name { get; set; }

    public static implicit operator AzureTableItem(TableItem d)
    {
        return new(d);
    }

    public static implicit operator TableItem(AzureTableItem d)
    {
        return new TableItem(d.Name);
    }

    public static Response<IAbstractTableItem> FromTableItem(Response<TableItem> response)
    {
        var x = new AzureTableItem(response.Value);
        return new AzureResponse<IAbstractTableItem>();
    }
}