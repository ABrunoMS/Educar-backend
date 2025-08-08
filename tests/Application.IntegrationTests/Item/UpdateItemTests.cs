using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Item.CreateItem;
using Educar.Backend.Application.Commands.Item.UpdateItem;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Item;

[TestFixture]
public class UpdateItemTests : TestBase
{
    private const string ValidUpdatedName = "Updated Item";
    private const string ValidUpdatedLore = "Updated Lore";
    private const string InitialItemName = "Initial Item";
    private const string InitialItemLore = "Initial Lore";
    private const ItemType InitialItemType = ItemType.Equipment;
    private const ItemRarity InitialItemRarity = ItemRarity.Common;
    private const decimal InitialSellValue = 100.00m;
    private const string InitialReference2D = "http://example.com/item2d.png";
    private const string InitialReference3D = "http://example.com/item3d.png";
    private const decimal InitialDropRate = 5.00m;

    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldUpdateItem()
    {
        // Arrange
        var createCommand = new CreateItemCommand(
            InitialItemName,
            InitialItemLore,
            InitialItemType,
            InitialItemRarity,
            InitialSellValue,
            InitialReference2D,
            InitialReference3D,
            InitialDropRate);

        var createResponse = await SendAsync(createCommand);

        var updateCommand = new UpdateItemCommand
        {
            Id = createResponse.Id,
            Name = ValidUpdatedName,
            Lore = ValidUpdatedLore,
            ItemType = ItemType.Common,
            ItemRarity = ItemRarity.Rare,
            SellValue = 200.00m,
            Reference2D = "http://example.com/updateditem2d.png",
            Reference3D = "http://example.com/updateditem3d.png",
            DropRate = 10.00m
        };

        // Act
        await SendAsync(updateCommand);

        // Assert
        var updatedItem = await Context.Items.FindAsync(updateCommand.Id);
        Assert.That(updatedItem, Is.Not.Null);
        Assert.That(updatedItem.Name, Is.EqualTo(ValidUpdatedName));
        Assert.That(updatedItem.Lore, Is.EqualTo(ValidUpdatedLore));
        Assert.That(updatedItem.ItemType, Is.EqualTo(ItemType.Common));
        Assert.That(updatedItem.ItemRarity, Is.EqualTo(ItemRarity.Rare));
        Assert.That(updatedItem.SellValue, Is.EqualTo(200.00m));
        Assert.That(updatedItem.Reference2D, Is.EqualTo("http://example.com/updateditem2d.png"));
        Assert.That(updatedItem.Reference3D, Is.EqualTo("http://example.com/updateditem3d.png"));
        Assert.That(updatedItem.DropRate, Is.EqualTo(10.00m));
    }

    [Test]
    public void ShouldThrowValidationException_WhenIdIsEmpty()
    {
        var command = new UpdateItemCommand
        {
            Id = Guid.Empty,
            Name = ValidUpdatedName,
            Lore = ValidUpdatedLore
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            Lore = ValidUpdatedLore
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenLoreIsEmpty()
    {
        var command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = ValidUpdatedName,
            Lore = string.Empty
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        var command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = ValidUpdatedName,
            Lore = ValidUpdatedLore
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var longName = new string('A', 151); // Name with 151 characters
        var command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = longName,
            Lore = ValidUpdatedLore
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenReference2DExceedsMaxLength()
    {
        var longReference2D = new string('A', 256); // Reference2D with 256 characters
        var command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = ValidUpdatedName,
            Lore = ValidUpdatedLore,
            Reference2D = longReference2D
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenReference3DExceedsMaxLength()
    {
        var longReference3D = new string('A', 256); // Reference3D with 256 characters
        var command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = ValidUpdatedName,
            Lore = ValidUpdatedLore,
            Reference3D = longReference3D
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenItemTypeIsInvalid()
    {
        var command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = ValidUpdatedName,
            Lore = ValidUpdatedLore,
            ItemType = (ItemType)999 // Invalid ItemType
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenItemRarityIsInvalid()
    {
        var command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = ValidUpdatedName,
            Lore = ValidUpdatedLore,
            ItemRarity = (ItemRarity)999 // Invalid ItemRarity
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenSellValueIsInvalid()
    {
        var command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = ValidUpdatedName,
            Lore = ValidUpdatedLore,
            SellValue = -100.00m // Invalid SellValue
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenDropRateIsInvalid()
    {
        var command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = ValidUpdatedName,
            Lore = ValidUpdatedLore,
            DropRate = -5.00m // Invalid DropRate
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}