using AutoFixture;
using Azure.Data.Tables;
using AzureTestAbstract.Helpers;
using AzureTestAbstract.Implementation;
using Moq;

namespace AzureTestAbstract;

public class TableConsumerTestsFixture : Fixture
{
    public Mock<IAbstractTableClient> MockTableClient { get; } = new();
    public Mock<IAbstractTableServiceClient> MockTableServiceClient { get; } = new();
    public Mock<IAbstractTableItem> MockAbstractTableItem { get; } = new();

    public TableConsumer GetSut() => new TableConsumer(MockTableClient.Object, MockTableServiceClient.Object);

    public IEnumerable<AzureTableItem> Create_AzureTableItems() =>
        Build<AzureTableItem>()
            .With(x => x.Name)
            .CreateMany();

    public TableConsumerTestsFixture With_TableClient_GetEntity_ReturningObject()
    {
        MockTableClient.Setup(s =>
                s.GetEntity<AzureTableEntity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                 .Returns(new AzureResponse<AzureTableEntity>());
        
        return this;
    }
}