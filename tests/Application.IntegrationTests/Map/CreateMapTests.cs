using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Game.CreateGame;
using Educar.Backend.Application.Commands.Map.CreateMap;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Map;

[TestFixture]
public class CreateMapTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldCreateMap()
    {
        const string name = "New Map";
        const string description = "Map Description";
        const string reference2D = "Reference 2D";
        const string reference3D = "Reference 3D";

        // Arrange
        var createGameCommand = new CreateGameCommand("Test Game", "Game Description", "Lore", "Purpose");
        var createdGameResponse = await SendAsync(createGameCommand);

        var command = new CreateMapCommand(name, description, MapType.World, reference2D, reference3D,
            createdGameResponse.Id);

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<IdResponseDto>());

        var createdMap = await Context.Maps
            .Include(m => m.Game)
            .FirstOrDefaultAsync(m => m.Id == response.Id);
        Assert.That(createdMap, Is.Not.Null);
        Assert.That(createdMap.Name, Is.EqualTo(name));
        Assert.That(createdMap.Description, Is.EqualTo(description));
        Assert.That(createdMap.Reference2D, Is.EqualTo(reference2D));
        Assert.That(createdMap.Reference3D, Is.EqualTo(reference3D));
        Assert.That(createdMap.GameId, Is.EqualTo(createdGameResponse.Id));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new CreateMapCommand(string.Empty, "Map Description", MapType.World, "Reference 2D",
            "Reference 3D", Guid.NewGuid());

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var longName = new string('a', 151);
        var command = new CreateMapCommand(longName, "Map Description", MapType.World, "Reference 2D", "Reference 3D",
            Guid.NewGuid());

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenDescriptionIsEmpty()
    {
        var command = new CreateMapCommand("Map Name", string.Empty, MapType.World, "Reference 2D", "Reference 3D",
            Guid.NewGuid());

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenTypeIsInvalid()
    {
        var command = new CreateMapCommand("Map Name", "Map Description", MapType.None, "Reference 2D", "Reference 3D",
            Guid.NewGuid());

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenReference2DIsEmpty()
    {
        var command = new CreateMapCommand("Map Name", "Map Description", MapType.World, string.Empty, "Reference 3D",
            Guid.NewGuid());

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenReference3DIsEmpty()
    {
        var command = new CreateMapCommand("Map Name", "Map Description", MapType.World, "Reference 2D", string.Empty,
            Guid.NewGuid());

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowNotFoundException_WhenGameIdIsInvalid()
    {
        var command = new CreateMapCommand("Map Name", "Map Description", MapType.World, "Reference 2D", "Reference 3D",
            Guid.NewGuid());

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }
}   