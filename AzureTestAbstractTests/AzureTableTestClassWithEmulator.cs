using AutoFixture.Xunit2;
using Azure.Data.Tables;
using AzureTestAbstract.Implementation;
using Moq;
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

    [Theory]
    [AutoData]
    public void T_AddEntity(TableConsumerTestsFixture f, string pkey, string rkey, string stringValue, int intValue, double doubleValue,
        decimal decimalValue)
    {
        // arrange
        var accountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";
        var accountName = "devstoreaccount1";
        var url = "http://127.0.0.1:10002/devstoreaccount1";

        var azureTableClient =
            new AzureTableClient(new Uri(url), "tableName", new TableSharedKeyCredential(accountName, accountKey));
        var serviceClient = new AzureTableServiceClient(url,
            new TableSharedKeyCredential("devstoreaccount1",
                "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="));


        var listOfTableConsumerTestClass = f.TableConsumerTestClass(5).ToList();
        var sut = new TableConsumer(azureTableClient, serviceClient);

        // act
        var response = sut.AddEntity(pkey, rkey, stringValue, intValue, doubleValue, decimalValue, listOfTableConsumerTestClass);
        var actual = sut.T_GetEntity(pkey, rkey);

        // assert

        var x = actual["decimal"].ToString();

        Assert.NotNull(actual);
        Assert.Equal(actual["string"], stringValue);
        Assert.Equal(actual.GetInt32("int"), intValue);
        Assert.Equal(actual.GetDouble("double"), doubleValue);
        Assert.Equal(decimal.Parse(x), decimalValue);
        Assert.Equal(actual["json"], Newtonsoft.Json.JsonConvert.SerializeObject(listOfTableConsumerTestClass));
    }

    [Theory]
    [AutoData]
    public async Task T_AddEntityAsync(TableConsumerTestsFixture f, string pkey, string rkey, string stringValue, int intValue, double doubleValue,
        decimal decimalValue)
    {
        // arrange
        var accountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";
        var accountName = "devstoreaccount1";
        var url = "http://127.0.0.1:10002/devstoreaccount1";

        var azureTableClient =
            new AzureTableClient(new Uri(url), "tableName", new TableSharedKeyCredential(accountName, accountKey));
        var serviceClient = new AzureTableServiceClient(url,
            new TableSharedKeyCredential("devstoreaccount1",
                "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="));


        var listOfTableConsumerTestClass = f.TableConsumerTestClass(5).ToList();
        var sut = new TableConsumer(azureTableClient, serviceClient);

        // act
        var response = await sut.AddEntityAsync(pkey, rkey, stringValue, intValue, doubleValue, decimalValue, listOfTableConsumerTestClass, default);
        var actual = sut.T_GetEntity(pkey, rkey);

        // assert
        var x = actual["decimal"].ToString();

        Assert.NotNull(actual);
        Assert.Equal(actual["string"], stringValue);
        Assert.Equal(actual.GetInt32("int"), intValue);
        Assert.Equal(actual.GetDouble("double"), doubleValue);
        Assert.Equal(decimal.Parse(x), decimalValue);
        Assert.Equal(actual["json"], Newtonsoft.Json.JsonConvert.SerializeObject(listOfTableConsumerTestClass));
    }
}