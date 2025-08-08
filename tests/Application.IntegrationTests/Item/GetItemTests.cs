using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Item.CreateItem;
using Educar.Backend.Application.Queries.Item;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Item;

[TestFixture]
public class GetItemTests : TestBase
{
    private const string ItemName = "Test Item";
    private const string ItemLore = "Test Lore";
    private const ItemType ItemType = Domain.Enums.ItemType.Equipment;
    private const ItemRarity ItemRarity = Domain.Enums.ItemRarity.Common;
    private const decimal SellValue = 100.00m;
    private const string Reference2D = "http://example.com/item2d.png";
    private const string Reference3D = "http://example.com/item3d.png";
    private const decimal DropRate = 5.00m;

    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldReturnItem()
    {
        // Arrange
        var createCommand = new CreateItemCommand(
            ItemName,
            ItemLore,
            ItemType,
            ItemRarity,
            SellValue,
            Reference2D,
            Reference3D,
            DropRate);

        var createResponse = await SendAsync(createCommand);

        var query = new GetItemQuery { Id = createResponse.Id };

        // Act
        var itemResponse = await SendAsync(query);

        // Assert
        Assert.That(itemResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(itemResponse.Id, Is.EqualTo(createResponse.Id));
            Assert.That(itemResponse.Name, Is.EqualTo(ItemName));
            Assert.That(itemResponse.Lore, Is.EqualTo(ItemLore));
            Assert.That(itemResponse.ItemType, Is.EqualTo(ItemType));
            Assert.That(itemResponse.ItemRarity, Is.EqualTo(ItemRarity));
            Assert.That(itemResponse.SellValue, Is.EqualTo(SellValue));
            Assert.That(itemResponse.Reference2D, Is.EqualTo(Reference2D));
            Assert.That(itemResponse.Reference3D, Is.EqualTo(Reference3D));
            Assert.That(itemResponse.DropRate, Is.EqualTo(DropRate));
        });
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetItemQuery { Id = Guid.NewGuid() };

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenValidPaginationRequest_ShouldReturnPaginatedItems()
    {
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var command = new CreateItemCommand(
                $"Test Item {i}",
                ItemLore,
                ItemType,
                ItemRarity,
                SellValue,
                Reference2D,
                Reference3D,
                DropRate);
            await SendAsync(command);
        }

        var query = new GetItemsByNamePaginatedQuery("Test") { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(10));
            Assert.That(result.PageNumber, Is.EqualTo(1));
            Assert.That(result.TotalCount, Is.EqualTo(20));
            Assert.That(result.TotalPages, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GivenSpecificPageRequest_ShouldReturnCorrectPage()
    {
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var command = new CreateItemCommand(
                $"Test Item {i}",
                ItemLore,
                ItemType,
                ItemRarity,
                SellValue,
                Reference2D,
                Reference3D,
                DropRate);
            await SendAsync(command);
        }

        var query = new GetItemsByNamePaginatedQuery("Test") { PageNumber = 2, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(10));
            Assert.That(result.PageNumber, Is.EqualTo(2));
            Assert.That(result.TotalCount, Is.EqualTo(20));
            Assert.That(result.TotalPages, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GivenOutOfRangePageRequest_ShouldReturnEmptyPage()
    {
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var command = new CreateItemCommand(
                $"Test Item {i}",
                ItemLore,
                ItemType,
                ItemRarity,
                SellValue,
                Reference2D,
                Reference3D,
                DropRate);
            await SendAsync(command);
        }

        var query = new GetItemsByNamePaginatedQuery("Test") { PageNumber = 3, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Is.Empty);
            Assert.That(result.PageNumber, Is.EqualTo(3));
            Assert.That(result.TotalCount, Is.EqualTo(20));
            Assert.That(result.TotalPages, Is.EqualTo(2));
        });
    }
}