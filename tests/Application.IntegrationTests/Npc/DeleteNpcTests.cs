using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Item.CreateItem;
using Educar.Backend.Application.Commands.Npc.CreateNpc;
using Educar.Backend.Application.Commands.Npc.DeleteNpc;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Npc;

[TestFixture]
public class DeleteNpcTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidId_ShouldDeleteNpc()
    {
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

        var createNpcCommand = new CreateNpcCommand("Test Npc", "Npc Lore", NpcType.Boss, 5.00m, 100.00m)
        {
            ItemIds = new List<Guid> { createdItemResponse.Id }
        };
        var createdNpcResponse = await SendAsync(createNpcCommand);
        var npcId = createdNpcResponse.Id;

        var deleteCommand = new DeleteNpcCommand(npcId);

        // Act
        await SendAsync(deleteCommand);

        // Assert
        var deletedNpc = await Context.Npcs.IgnoreQueryFilters().FirstOrDefaultAsync(n => n.Id == npcId);
        Assert.That(deletedNpc, Is.Not.Null);
        Assert.That(deletedNpc.IsDeleted, Is.True);

        // Ensure the associated items are also cleared
        var deletedNpcItems = await Context.NpcItems.IgnoreQueryFilters().Where(ni => ni.NpcId == npcId).ToListAsync();
        Assert.That(deletedNpcItems.All(ni => ni.IsDeleted), Is.True);
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        var deleteCommand = new DeleteNpcCommand(Guid.NewGuid());

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(deleteCommand));
    }
}