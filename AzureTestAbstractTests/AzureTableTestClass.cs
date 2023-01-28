using AutoFixture;
using AutoFixture.Xunit2;
using Azure;
using Azure.Data.Tables;
using AzureTestAbstract.Implementation;
using Moq;
using Xunit;

namespace AzureTestAbstract;

public class AzureTableTestClass
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
    public void T_With_Mock(Fixture f, string mockTableItem_Name)
    {
        // arrange
        var mockTableClient = new Mock<IAbstractTableClient>();
        var mockTableServiceClient = new Mock<IAbstractTableServiceClient>();
        var mockTableItem = new Mock<IAbstractTableItem>();


        var sut = new TableConsumer(mockTableClient.Object, mockTableServiceClient.Object);

        mockTableItem.Setup(s => s.Name)
            .Returns(mockTableItem_Name);

        mockTableServiceClient
            .Setup(s => s.CreateTableIfNotExists(It.IsAny<string>()))
            .Returns(mockTableItem.Object);


        var abstractTableItems = f.Build<AzureTableItem>()
            .With(x => x.Name)
            .CreateMany();

        var pages = new List<Page<IAbstractTableItem>>
        {
            Page<IAbstractTableItem>.FromValues(abstractTableItems.ToList(), null, null)
        };

        var pageable = Pageable<IAbstractTableItem>.FromPages(pages);

        mockTableServiceClient
            .Setup(s => s.Query(It.IsAny<string>()))
            .Returns(pageable);

        var abstractTableEntities = f.Build<AzureTableEntity>()
            .CreateMany();

        var pagesEntities = new List<Page<AzureTableEntity>>
        {
            Page<AzureTableEntity>.FromValues(abstractTableEntities.ToList(), null, null)
        };

        var pageableEntities = Pageable<AzureTableEntity>.FromPages(pagesEntities);


        mockTableClient
            .Setup(s => s.Query<AzureTableEntity>(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()))
            .Returns(pageableEntities);

        // act

        sut.WorkTest();

        // assert
    }
}