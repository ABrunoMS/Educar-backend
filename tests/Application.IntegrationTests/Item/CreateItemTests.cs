using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Item.CreateItem;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Item;

[TestFixture]
public class CreateItemTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldCreateItem()
    {
        // Arrange
        var command = new CreateItemCommand(
            "New Item",
            "Item Lore",
            ItemType.Equipment,
            ItemRarity.Common,
            100.00m,
            "http://example.com/item2d.png",
            "http://example.com/item3d.png",
            5.00m);

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<IdResponseDto>());

        var createdItem = await Context.Items.FindAsync(response.Id);
        Assert.That(createdItem, Is.Not.Null);
        Assert.That(createdItem.Name, Is.EqualTo("New Item"));
        Assert.That(createdItem.Lore, Is.EqualTo("Item Lore"));
        Assert.That(createdItem.ItemType, Is.EqualTo(ItemType.Equipment));
        Assert.That(createdItem.ItemRarity, Is.EqualTo(ItemRarity.Common));
        Assert.That(createdItem.SellValue, Is.EqualTo(100.00m));
        Assert.That(createdItem.Reference2D, Is.EqualTo("http://example.com/item2d.png"));
        Assert.That(createdItem.Reference3D, Is.EqualTo("http://example.com/item3d.png"));
        Assert.That(createdItem.DropRate, Is.EqualTo(5.00m));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new CreateItemCommand(
            string.Empty,
            "Item Lore",
            ItemType.Equipment,
            ItemRarity.Common,
            100.00m,
            "http://example.com/item2d.png",
            "http://example.com/item3d.png",
            5.00m);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenLoreIsEmpty()
    {
        var command = new CreateItemCommand(
            "New Item",
            string.Empty,
            ItemType.Equipment,
            ItemRarity.Common,
            100.00m,
            "http://example.com/item2d.png",
            "http://example.com/item3d.png",
            5.00m);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var longName = new string('A', 151); // Name with 151 characters
        var command = new CreateItemCommand(
            longName,
            "Item Lore",
            ItemType.Equipment,
            ItemRarity.Common,
            100.00m,
            "http://example.com/item2d.png",
            "http://example.com/item3d.png",
            5.00m);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenReference2DExceedsMaxLength()
    {
        var longReference2D = new string('A', 256); // Reference2D with 256 characters
        var command = new CreateItemCommand(
            "New Item",
            "Item Lore",
            ItemType.Equipment,
            ItemRarity.Common,
            100.00m,
            longReference2D,
            "http://example.com/item3d.png",
            5.00m);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenReference3DExceedsMaxLength()
    {
        var longReference3D = new string('A', 256); // Reference3D with 256 characters
        var command = new CreateItemCommand(
            "New Item",
            "Item Lore",
            ItemType.Equipment,
            ItemRarity.Common,
            100.00m,
            "http://example.com/item2d.png",
            longReference3D,
            5.00m);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenItemTypeIsInvalid()
    {
        var command = new CreateItemCommand(
            "New Item",
            "Item Lore",
            (ItemType)999, // Invalid ItemType
            ItemRarity.Common,
            100.00m,
            "http://example.com/item2d.png",
            "http://example.com/item3d.png",
            5.00m);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenItemRarityIsInvalid()
    {
        var command = new CreateItemCommand(
            "New Item",
            "Item Lore",
            ItemType.Equipment,
            (ItemRarity)999, // Invalid ItemRarity
            100.00m,
            "http://example.com/item2d.png",
            "http://example.com/item3d.png",
            5.00m);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}