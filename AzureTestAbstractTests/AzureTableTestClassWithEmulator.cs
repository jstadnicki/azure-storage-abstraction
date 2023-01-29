using Azure.Data.Tables;
using AzureTestAbstract.Implementation;
using Xunit;

namespace AzureTestAbstract;

public class AzureTableTestClassWithEmulator
{
    [Fact]
    public void T_With_Emulator()
    {
        // arrange
        var pKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";
        var accountName = "devstoreaccount1";
        var url = "http://127.0.0.1:10002/devstoreaccount1";

        var azureTableClient =
            new AzureTableClient(new Uri(url), "tableName", new TableSharedKeyCredential(accountName, pKey));
        var serviceClient = new AzureTableServiceClient(url,
            new TableSharedKeyCredential("devstoreaccount1",
                "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="));

        // act
        var sut = new TableConsumer(azureTableClient, serviceClient);

        // assert
        sut.WorkTest();
    }
}