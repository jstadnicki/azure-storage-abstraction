using Azure;
using AzureTestAbstract.Implementation;

namespace AzureTestAbstract.Helpers;

public class AzureResponse<T> : Response<T>
{
    public AzureResponse(T value)
    {
        Value = value;
    }

    public override T Value { get; }

    public override Response GetRawResponse()
    {
        throw new NotImplementedException();
    }
}