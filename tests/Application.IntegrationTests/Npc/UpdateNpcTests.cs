using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Item.CreateItem;
using Educar.Backend.Application.Commands.Npc.CreateNpc;
using Educar.Backend.Application.Commands.Npc.UpdateNpc;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Npc;

[TestFixture]
public class UpdateNpcTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldUpdateNpc()
    {
        const string originalName = "Original Name";
        const string originalLore = "Original Lore";
        const NpcType originalNpcType = NpcType.Boss;
        const decimal originalGoldDropRate = 5.00m;
        const decimal originalGoldAmount = 100.00m;

        const string updatedName = "Updated Name";
        const string updatedLore = "Updated Lore";
        const NpcType updatedNpcType = NpcType.Common;
        const decimal updatedGoldDropRate = 10.00m;
        const decimal updatedGoldAmount = 200.00m;

        // Arrange
        var createItemCommand = new CreateItemCommand(
            "Test Item",
            "Item Lore",
            ItemType.Equipment,
            ItemRarity.Common,
            50.00m,
            "http://example.com/item2d.png",
            "http://example.com/item3d.png",
            10.00m);
        var createdItemResponse = await SendAsync(createItemCommand);

        var createNpcCommand = new CreateNpcCommand(originalName, originalLore, originalNpcType, originalGoldDropRate,
            originalGoldAmount)
        {
            ItemIds = new List<Guid> { createdItemResponse.Id }
        };
        var createdNpcResponse = await SendAsync(createNpcCommand);
        var npcId = createdNpcResponse.Id;

        var updateItemCommand = new CreateItemCommand(
            "Updated Item",
            "Updated Item Lore",
            ItemType.Equipment,
            ItemRarity.Common,
            50.00m,
            "http://example.com/item2dupdated.png",
            "http://example.com/item3dupdated.png",
            10.00m);
        var updatedItemResponse = await SendAsync(updateItemCommand);

        var updateCommand = new UpdateNpcCommand
        {
            Id = npcId,
            Name = updatedName,
            Lore = updatedLore,
            NpcType = updatedNpcType,
            GoldDropRate = updatedGoldDropRate,
            GoldAmount = updatedGoldAmount,
            ItemIds = new List<Guid> { updatedItemResponse.Id }
        };

        // Act
        await SendAsync(updateCommand);

        // Assert
        if (Context.Npcs != null)
        {
            var updatedNpc = await Context.Npcs
                .Include(n => n.NpcItems)
                .ThenInclude(ni => ni.Item)
                .FirstOrDefaultAsync(n => n.Id == npcId);
            Assert.That(updatedNpc, Is.Not.Null);
            Assert.That(updatedNpc.Name, Is.EqualTo(updatedName));
            Assert.That(updatedNpc.Lore, Is.EqualTo(updatedLore));
            Assert.That(updatedNpc.NpcType, Is.EqualTo(updatedNpcType));
            Assert.That(updatedNpc.GoldDropRate, Is.EqualTo(updatedGoldDropRate));
            Assert.That(updatedNpc.GoldAmount, Is.EqualTo(updatedGoldAmount));
            Assert.That(updatedNpc.NpcItems, Has.Count.EqualTo(1));
            Assert.That(updatedNpc.NpcItems.First().Item.Name, Is.EqualTo("Updated Item"));
        }
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        var updateCommand = new UpdateNpcCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Name",
            Lore = "Updated Lore",
            NpcType = NpcType.Common,
            GoldDropRate = 10.00m,
            GoldAmount = 200.00m
        };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var updateCommand = new UpdateNpcCommand
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            Lore = "Updated Lore",
            NpcType = NpcType.Common,
            GoldDropRate = 10.00m,
            GoldAmount = 200.00m
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var longName = new string('a', 151);
        var updateCommand = new UpdateNpcCommand
        {
            Id = Guid.NewGuid(),
            Name = longName,
            Lore = "Updated Lore",
            NpcType = NpcType.Common,
            GoldDropRate = 10.00m,
            GoldAmount = 200.00m
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenLoreIsEmpty()
    {
        var updateCommand = new UpdateNpcCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Name",
            Lore = string.Empty,
            NpcType = NpcType.Common,
            GoldDropRate = 10.00m,
            GoldAmount = 200.00m
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNpcTypeIsInvalid()
    {
        var updateCommand = new UpdateNpcCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Name",
            Lore = "Updated Lore",
            NpcType = (NpcType)999,
            GoldDropRate = 10.00m,
            GoldAmount = 200.00m
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }
}