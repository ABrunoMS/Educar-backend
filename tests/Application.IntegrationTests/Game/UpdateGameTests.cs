using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Game.CreateGame;
using Educar.Backend.Application.Commands.Game.UpdateGame;
using Educar.Backend.Application.Common.Exceptions;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Game;

[TestFixture]
public class UpdateGameTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldUpdateGame()
    {
        // Arrange
        var createCommand =
            new CreateGameCommand("Original Name", "Original Description", "Original Lore", "Original Purpose");
        var createdResponse = await SendAsync(createCommand);
        var gameId = createdResponse.Id;

        var updateCommand = new UpdateGameCommand
        {
            Id = gameId,
            Name = "Updated Name",
            Description = "Updated Description",
            Lore = "Updated Lore",
            Purpose = "Updated Purpose"
        };

        // Act
        await SendAsync(updateCommand);

        // Assert
        if (Context.Games != null)
        {
            var updatedGame = await Context.Games.FindAsync(gameId);
            Assert.That(updatedGame, Is.Not.Null);
            Assert.That(updatedGame.Name, Is.EqualTo("Updated Name"));
            Assert.That(updatedGame.Description, Is.EqualTo("Updated Description"));
            Assert.That(updatedGame.Lore, Is.EqualTo("Updated Lore"));
            Assert.That(updatedGame.Purpose, Is.EqualTo("Updated Purpose"));
        }
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        var updateCommand = new UpdateGameCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Name",
            Description = "Updated Description",
            Lore = "Updated Lore",
            Purpose = "Updated Purpose"
        };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var updateCommand = new UpdateGameCommand
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            Description = "Updated Description",
            Lore = "Updated Lore",
            Purpose = "Updated Purpose"
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var longName = new string('a', 101);
        var updateCommand = new UpdateGameCommand
        {
            Id = Guid.NewGuid(),
            Name = longName,
            Description = "Updated Description",
            Lore = "Updated Lore",
            Purpose = "Updated Purpose"
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenDescriptionIsEmpty()
    {
        var updateCommand = new UpdateGameCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Name",
            Description = string.Empty,
            Lore = "Updated Lore",
            Purpose = "Updated Purpose"
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenLoreIsEmpty()
    {
        var updateCommand = new UpdateGameCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Name",
            Description = "Updated Description",
            Lore = string.Empty,
            Purpose = "Updated Purpose"
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenPurposeIsEmpty()
    {
        var updateCommand = new UpdateGameCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Name",
            Description = "Updated Description",
            Lore = "Updated Lore",
            Purpose = string.Empty
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenPurposeExceedsMaxLength()
    {
        var longPurpose = new string('a', 256);
        var updateCommand = new UpdateGameCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Name",
            Description = "Updated Description",
            Lore = "Updated Lore",
            Purpose = longPurpose
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }
}