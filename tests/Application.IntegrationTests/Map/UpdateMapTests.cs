using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Map.CreateMap;
using Educar.Backend.Application.Commands.Map.UpdateMap;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Map;

[TestFixture]
public class UpdateMapCommandTests : TestBase
{
    private Domain.Entities.Game _game;
    private Domain.Entities.Map _map;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _game = new Domain.Entities.Game("Test Game", "Game Description", "lore", "purpose");
        Context.Games.Add(_game);
        Context.SaveChanges();

        _map = new Domain.Entities.Map("Initial Map", "Initial Description", MapType.World, "Initial2D", "Initial3D")
        {
            Game = _game
        };
        Context.Maps.Add(_map);
        Context.SaveChanges();
    }

    [Test]
    public async Task GivenValidRequest_ShouldUpdateMap()
    {
        var mapCommand = new CreateMapCommand("Map Name", "Map Description", MapType.World, "Reference 2D",
            "Reference 3D", _game.Id);
        var mapResponse = await SendAsync(mapCommand);

        // Arrange
        const string newName = "Updated Map";
        const string newDescription = "Updated Description";
        const string newReference2D = "Updated2D";
        const string newReference3D = "Updated3D";
        const MapType newType = MapType.Dungeon;

        var command = new UpdateMapCommand(mapResponse.Id, newName, newDescription, newType, newReference2D,
            newReference3D);

        // Act
        await SendAsync(command);

        // Assert
        var updatedMap = await Context.Maps.FirstOrDefaultAsync(m => m.Id == mapResponse.Id);

        Assert.That(updatedMap, Is.Not.Null);
        Assert.That(updatedMap.Name, Is.EqualTo(newName));
        Assert.That(updatedMap.Description, Is.EqualTo(newDescription));
        Assert.That(updatedMap.Type, Is.EqualTo(newType));
        Assert.That(updatedMap.Reference2D, Is.EqualTo(newReference2D));
        Assert.That(updatedMap.Reference3D, Is.EqualTo(newReference3D));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new UpdateMapCommand(_map.Id, string.Empty, "Map Description", MapType.World, "Reference 2D",
            "Reference 3D");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var longName = new string('a', 151);
        var command = new UpdateMapCommand(_map.Id, longName, "Map Description", MapType.World, "Reference 2D",
            "Reference 3D");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenDescriptionIsEmpty()
    {
        var command = new UpdateMapCommand(_map.Id, "Map Name", string.Empty, MapType.World, "Reference 2D",
            "Reference 3D");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenTypeIsInvalid()
    {
        var command = new UpdateMapCommand(_map.Id, "Map Name", "Map Description", MapType.None, "Reference 2D",
            "Reference 3D");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenReference2DIsEmpty()
    {
        var command = new UpdateMapCommand(_map.Id, "Map Name", "Map Description", MapType.World, string.Empty,
            "Reference 3D");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenReference3DIsEmpty()
    {
        var command = new UpdateMapCommand(_map.Id, "Map Name", "Map Description", MapType.World, "Reference 2D",
            string.Empty);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenIdIsEmpty()
    {
        var command = new UpdateMapCommand(Guid.Empty, "Map Name", "Map Description", MapType.World, "Reference 2D",
            "Reference 3D");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowNotFoundException_WhenIdIsInvalid()
    {
        var command = new UpdateMapCommand(Guid.NewGuid(), "Map Name", "Map Description", MapType.World, "Reference 2D",
            "Reference 3D");

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }
}