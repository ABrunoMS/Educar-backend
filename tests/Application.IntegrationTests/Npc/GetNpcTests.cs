using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Item.CreateItem;
using Educar.Backend.Application.Commands.Npc.CreateNpc;
using Educar.Backend.Application.Queries.Npc;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Npc;

[TestFixture]
public class GetNpcTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidId_ShouldReturnNpc()
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

        var query = new GetNpcQuery { Id = npcId };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Test Npc"));
        Assert.That(result.Lore, Is.EqualTo("Npc Lore"));
        Assert.That(result.NpcType, Is.EqualTo(NpcType.Boss));
        Assert.That(result.GoldDropRate, Is.EqualTo(5.00m));
        Assert.That(result.GoldAmount, Is.EqualTo(100.00m));
        Assert.That(result.Items, Is.Not.Empty);
        Assert.That(result.Items.First().Name, Is.EqualTo("Test Item"));
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        var query = new GetNpcQuery { Id = Guid.NewGuid() };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenPageAndPageSize_ShouldReturnPaginatedNpcs()
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

        for (var i = 1; i <= 20; i++)
        {
            var command = new CreateNpcCommand($"Test Npc {i}", "Npc Lore", NpcType.Boss, 5.00m, 100.00m)
            {
                ItemIds = new List<Guid> { createdItemResponse.Id }
            };
            await SendAsync(command);
        }

        var query = new GetNpcsByNamePaginatedQuery("Test") { PageNumber = 1, PageSize = 10 };

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
    public async Task GivenPageAndPageSize_ShouldReturnCorrectPage()
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

        for (var i = 1; i <= 2; i++)
        {
            var command = new CreateNpcCommand($"Test Npc {i}", "Npc Lore", NpcType.Boss, 5.00m, 100.00m)
            {
                ItemIds = new List<Guid> { createdItemResponse.Id }
            };
            await SendAsync(command);
        }

        var query = new GetNpcsByNamePaginatedQuery("Test") { PageNumber = 2, PageSize = 1 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(1));
            Assert.That(result.PageNumber, Is.EqualTo(2));
            Assert.That(result.TotalCount, Is.EqualTo(2));
            Assert.That(result.TotalPages, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GivenPageAndPageSize_ShouldReturnEmptyWhenOutOfRange()
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

        for (var i = 1; i <= 20; i++)
        {
            var command = new CreateNpcCommand($"Test Npc {i}", "Npc Lore", NpcType.Boss, 5.00m, 100.00m)
            {
                ItemIds = new List<Guid> { createdItemResponse.Id }
            };
            await SendAsync(command);
        }

        var query = new GetNpcsByNamePaginatedQuery("Test") { PageNumber = 3, PageSize = 10 };

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