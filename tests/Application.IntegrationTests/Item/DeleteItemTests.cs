using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Item.CreateItem;
using Educar.Backend.Application.Commands.Item.DeleteItem;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Item;

[TestFixture]
public class DeleteItemTests : TestBase
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
    public async Task GivenValidRequest_ShouldDeleteItem()
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

        var deleteCommand = new DeleteItemCommand(createResponse.Id);

        // Act
        await SendAsync(deleteCommand);

        // Assert
        var deletedItem = await Context.Items.FirstOrDefaultAsync(i => i.Id == deleteCommand.Id);
        Assert.That(deletedItem, Is.Null);

        var softDeletedItem =
            await Context.Items.IgnoreQueryFilters().FirstOrDefaultAsync(i => i.Id == deleteCommand.Id);
        Assert.That(softDeletedItem, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(softDeletedItem.IsDeleted, Is.True);
            Assert.That(softDeletedItem.DeletedAt, Is.Not.Null);
        });
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        // Arrange
        var deleteCommand = new DeleteItemCommand(Guid.NewGuid());

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(deleteCommand));
    }
}