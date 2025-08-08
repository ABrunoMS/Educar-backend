using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Map.CreateMap;
using Educar.Backend.Application.Commands.Map.DeleteMap;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Map;

[TestFixture]
public class DeleteMapCommandTests : TestBase
{
    private Domain.Entities.Game _game;
    private Domain.Entities.Map _map;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _game = new Domain.Entities.Game("Test Game", "Game Description", "Lore", "Purpose");
        Context.Games.Add(_game);
        Context.SaveChanges();

        _map = new Domain.Entities.Map("Test Map", "Test Description", MapType.Dungeon, "Reference2D", "Reference3D")
        {
            Game = _game
        };
        Context.Maps.Add(_map);
        Context.SaveChanges();
    }

    [Test]
    public async Task GivenValidRequest_ShouldDeleteMap()
    {
        var createMapCommand = new CreateMapCommand("Test Map", "Test Description", MapType.Dungeon, "Reference2D",
            "Reference3D", _game.Id);
        var createMapResponse = await SendAsync(createMapCommand);

        // Arrange
        var deleteCommand = new DeleteMapCommand(createMapResponse.Id);

        // Act
        await SendAsync(deleteCommand);

        // Assert
        var deletedMap = await Context.Maps.IgnoreQueryFilters().FirstOrDefaultAsync(m => m.Id == createMapResponse.Id);
        Assert.That(deletedMap, Is.Not.Null);
        Assert.That(deletedMap.IsDeleted, Is.True);
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        // Arrange
        var deleteCommand = new DeleteMapCommand(Guid.NewGuid());

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(deleteCommand));
    }

    // [Test]
    // public async Task GivenMapWithSpawnPoints_ShouldDeleteMapAndClearSpawnPoints()
    // {
    //     // Arrange
    //     var spawnPoint = new SpawnPoint("Test Spawn", "Ref", 1m, 2m, 3m) { Map = _map };
    //     Context.SpawnPoints.Add(spawnPoint);
    //     await Context.SaveChangesAsync();
    //
    //     var deleteCommand = new DeleteMapCommand(_map.Id);
    //
    //     // Act
    //     await SendAsync(deleteCommand);
    //
    //     // Assert
    //     var deletedMap = await Context.Maps.IgnoreQueryFilters().FirstOrDefaultAsync(m => m.Id == _map.Id);
    //     Assert.That(deletedMap, Is.Null);
    //
    //     // Verify the associated SpawnPoints are deleted or detached
    //     var deletedSpawnPoint =
    //         await Context.SpawnPoints.IgnoreQueryFilters().FirstOrDefaultAsync(sp => sp.MapId == _map.Id);
    //     Assert.That(deletedSpawnPoint, Is.Null);
    // }
}