using Azure;

namespace AzureTestAbstract.Helpers;

public class AzureResponse<T> : Response<T>
{
    public override Response GetRawResponse()
    {
        throw new NotImplementedException();
    }

    public override T Value { get; }
}