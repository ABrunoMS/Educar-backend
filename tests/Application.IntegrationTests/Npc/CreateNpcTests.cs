using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Item.CreateItem;
using Educar.Backend.Application.Commands.Npc.CreateNpc;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Npc;

[TestFixture]
public class CreateNpcTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldCreateNpc()
    {
        const string name = "New Npc";
        const string lore = "Npc Lore";
        const NpcType npcType = NpcType.Common;
        const decimal goldDropRate = 5.00m;
        const decimal goldAmount = 100.00m;

        // Arrange
        var createItemCommand = new CreateItemCommand(
            "Test Item",
            "Item Lore",
            ItemType.Common,
            ItemRarity.Common,
            50.00m,
            "http://example.com/item2d.png",
            "http://example.com/item3d.png",
            10.00m);
        var createdItemResponse = await SendAsync(createItemCommand);

        var command = new CreateNpcCommand(name, lore, npcType, goldDropRate, goldAmount)
        {
            ItemIds = new List<Guid> { createdItemResponse.Id }
        };

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<CreatedResponseDto>());

        if (Context.Npcs != null)
        {
            var createdNpc = await Context.Npcs
                .Include(n => n.NpcItems)
                .ThenInclude(ni => ni.Item)
                .FirstOrDefaultAsync(n => n.Id == response.Id);
            Assert.That(createdNpc, Is.Not.Null);
            Assert.That(createdNpc.Name, Is.EqualTo(name));
            Assert.That(createdNpc.Lore, Is.EqualTo(lore));
            Assert.That(createdNpc.NpcType, Is.EqualTo(npcType));
            Assert.That(createdNpc.GoldDropRate, Is.EqualTo(goldDropRate));
            Assert.That(createdNpc.GoldAmount, Is.EqualTo(goldAmount));
            Assert.That(createdNpc.NpcItems, Has.Count.EqualTo(1));
            Assert.That(createdNpc.NpcItems.First().Item.Name, Is.EqualTo("Test Item"));
        }
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new CreateNpcCommand(string.Empty, "Npc Lore", NpcType.Boss, 5.00m, 100.00m);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var longName = new string('a', 151);
        var command = new CreateNpcCommand(longName, "Npc Lore", NpcType.Boss, 5.00m, 100.00m);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenLoreIsEmpty()
    {
        var command = new CreateNpcCommand("Npc Name", string.Empty, NpcType.Boss, 5.00m, 100.00m);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNpcTypeIsInvalid()
    {
        var command = new CreateNpcCommand("Npc Name", "Npc Lore", (NpcType)999, 5.00m, 100.00m);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}