using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Xunit2;
using Azure;
using Azure.Data.Tables;
using AzureTestAbstract.Implementation;
using Moq;
using Xunit;

namespace AzureTestAbstract;

public class AzureTableTestClassWithMock
{
    [Theory]
    [AutoData]
    public void T_With_Mock(TableConsumerTestsFixture f, string mockTableItem_Name)
    {
        // arrange
        var sut = f.GetSut();

        f.MockAbstractTableItem.Setup(s => s.Name)
            .Returns(mockTableItem_Name);

        f.MockTableServiceClient
            .Setup(s => s.CreateTableIfNotExists(It.IsAny<string>()))
            .Returns(f.MockAbstractTableItem.Object);


        var abstractTableItems = f.Create_AzureTableItems();


        var pages = new List<Page<IAbstractTableItem>>
        {
            Page<IAbstractTableItem>.FromValues(abstractTableItems.ToList(), null, null)
        };

        var pageable = Pageable<IAbstractTableItem>.FromPages(pages);

        f.MockTableServiceClient
            .Setup(s => s.Query(It.IsAny<string>()))
            .Returns(pageable);

        var abstractTableEntities = f.Build<AzureTableEntity>()
            .CreateMany();

        var pagesEntities = new List<Page<AzureTableEntity>>
        {
            Page<AzureTableEntity>.FromValues(abstractTableEntities.ToList(), null, null)
        };

        var pageableEntities = Pageable<AzureTableEntity>.FromPages(pagesEntities);


        f.MockTableClient
            .Setup(s => s.Query<AzureTableEntity>(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()))
            .Returns(pageableEntities);

        // act

        sut.WorkTest();

        // assert
    }

    [Theory]
    [AutoData]
    public void T_AddEntity(TableConsumerTestsFixture f, string pkey, string rkey)
    {
        // arrange
        var sut = f.GetSut();

        // act
        sut.AddEntity(pkey, rkey);

        // assert
        f.MockTableClient.Verify(v => v.AddEntity(It.IsAny<ITableEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task T_AddEntityAsync(TableConsumerTestsFixture f, string pkey, string rkey)
    {
        // arrange
        var sut = f.GetSut();

        // act
        await sut.AddEntityAsync(pkey, rkey);

        // assert
        f.MockTableClient.Verify(v => v.AddEntityAsync(It.IsAny<ITableEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task T_Query(TableConsumerTestsFixture f)
    {
        // arrange
        var sut = f.GetSut();

        // act
        sut.Query();

        // assert
        f.MockTableClient
            .Verify(
                v => v.Query(
                    It.IsAny<Expression<Func<AzureTableEntity, bool>>>(),
                    It.IsAny<int?>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
    }

    [Theory]
    [AutoData]
    public async Task T_QueryWithString(TableConsumerTestsFixture f, string pkey, string rkey)
    {
        // arrange
        var sut = f.GetSut();

        // act
        sut.QueryWithString();

        // assert
        f.MockTableClient
            .Verify(
                v => v.Query<AzureTableEntity>(
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
    }

    [Theory]
    [AutoData]
    public async Task T_Create(TableConsumerTestsFixture f, string pkey, string rkey)
    {
        // arrange
        var sut = f.GetSut();

        // act
        var actual = sut.T_Create();

        // assert
        f.MockTableClient.Verify(v => v.Create(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task T_CreateAsync(TableConsumerTestsFixture f, string pkey, string rkey)
    {
        // arrange
        var sut = f.GetSut();

        // act
        var actual = await sut.T_CreateAsync();

        // assert
        f.MockTableClient.Verify(v => v.CreateAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task T_DeleteAsync(TableConsumerTestsFixture f, string pkey, string rkey)
    {
        // arrange
        var sut = f.GetSut();

        // act
        var actual = await sut.T_DeleteAsync();

        // assert
        f.MockTableClient.Verify(v => v.DeleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task T_Delete(TableConsumerTestsFixture f, string pkey, string rkey)
    {
        // arrange
        var sut = f.GetSut();

        // act
        var actual = sut.T_Delete();

        // assert
        f.MockTableClient.Verify(v => v.Delete(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task T_GetEntity(TableConsumerTestsFixture f, string pkey, string rkey)
    {
        // arrange
        var sut = f
            .With_TableClient_GetEntity_ReturningObject()
            .GetSut();

        // act
        var actual = sut.T_GetEntity(pkey, rkey);

        // assert
        f.MockTableClient.Verify(
            v => v.GetEntity<AzureTableEntity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Theory]
    [AutoData]
    public async Task T_GetEntityAsync(TableConsumerTestsFixture f, string pkey, string rkey)
    {
        // arrange
        var sut = f.GetSut();

        // act
        var actual = await sut.T_GetEntityAsync(pkey, rkey);

        // assert
        f.MockTableClient.Verify(
            v => v.GetEntityAsync<AzureTableEntity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    
    [Theory]
    [AutoData]
    public async Task T_GetEntityIfExists(TableConsumerTestsFixture f, string pkey, string rkey)
    {
        // arrange
        var sut = f.GetSut();

        // act
        var actual = sut.T_GetEntityIfExists(pkey, rkey);

        // assert
        f.MockTableClient.Verify(
            v => v.GetEntityIfExists<AzureTableEntity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Theory]
    [AutoData]
    public async Task T_GetEntityIfExistsAsync(TableConsumerTestsFixture f, string pkey, string rkey)
    {
        // arrange
        var sut = f.GetSut();

        // act
        var actual = await sut.T_GetEntityIfExistsAsync(pkey, rkey);

        // assert
        f.MockTableClient.Verify(
            v => v.GetEntityIfExistsAsync<AzureTableEntity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Theory]
    [AutoData]
    public async Task T_SubmitTransaction(TableConsumerTestsFixture f, string pkey, string rkey)
    {
        // arrange
        var sut = f.GetSut();

        // act
        var actual = sut.T_SubmitTransaction();

        // assert
        f.MockTableClient.Verify(
            v => v.SubmitTransaction(It.IsAny<IEnumerable<TableTransactionAction>>(),It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Theory]
    [AutoData]
    public async Task T_SubmitTransactionAsync(TableConsumerTestsFixture f, string pkey, string rkey)
    {
        // arrange
        var sut = f.GetSut();

        // act
        var actual = await sut.T_SubmitTransactionAsync();

        // assert
        f.MockTableClient.Verify(
            v => v.SubmitTransactionAsync(It.IsAny<IEnumerable<TableTransactionAction>>(),It.IsAny<CancellationToken>()),
            Times.Once);
    }
}