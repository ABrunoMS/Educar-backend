using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Game.CreateGame;
using Educar.Backend.Application.Commands.Subject.CreateSubject;
using Educar.Backend.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Game;

[TestFixture]
public class CreateGameTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldCreateGame()
    {
        const string name = "New Game";
        const string description = "Game Description";
        const string lore = "Game Lore";
        const string purpose = "Game Purpose";

        // Arrange
        var createSubjectCommand = new CreateSubjectCommand("Test Subject", "Subject Description");
        var createdSubjectResponse = await SendAsync(createSubjectCommand);

        var command = new CreateGameCommand(name, description, lore, purpose)
        {
            SubjectIds = new List<Guid> { createdSubjectResponse.Id }
        };

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<CreatedResponseDto>());

        if (Context.Games != null)
        {
            var createdGame = await Context.Games
                .Include(g => g.GameSubjects)
                .ThenInclude(gs => gs.Subject)
                .FirstOrDefaultAsync(g => g.Id == response.Id);
            Assert.That(createdGame, Is.Not.Null);
            Assert.That(createdGame.Name, Is.EqualTo(name));
            Assert.That(createdGame.Description, Is.EqualTo(description));
            Assert.That(createdGame.Lore, Is.EqualTo(lore));
            Assert.That(createdGame.Purpose, Is.EqualTo(purpose));
            Assert.That(createdGame.GameSubjects, Has.Count.EqualTo(1));
            Assert.That(createdGame.GameSubjects.First().Subject.Name, Is.EqualTo("Test Subject"));
        }
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new CreateGameCommand(string.Empty, "Game Description", "Game Lore", "Game Purpose");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var longName = new string('a', 101);
        var command = new CreateGameCommand(longName, "Game Description", "Game Lore", "Game Purpose");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenNameIsNotUnique()
    {
        // Arrange
        var command1 = new CreateGameCommand("Unique Game", "Game Description", "Game Lore", "Game Purpose");
        await SendAsync(command1);

        // Act
        var command2 = new CreateGameCommand("Unique Game", "Another Description", "Another Lore", "Another Purpose");

        // Assert
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command2));
    }

    [Test]
    public void ShouldThrowValidationException_WhenDescriptionIsEmpty()
    {
        var command = new CreateGameCommand("Game Name", string.Empty, "Game Lore", "Game Purpose");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenLoreIsEmpty()
    {
        var command = new CreateGameCommand("Game Name", "Game Description", string.Empty, "Game Purpose");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenPurposeIsEmpty()
    {
        var command = new CreateGameCommand("Game Name", "Game Description", "Game Lore", string.Empty);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenPurposeExceedsMaxLength()
    {
        var longPurpose = new string('a', 256);
        var command = new CreateGameCommand("Game Name", "Game Description", "Game Lore", longPurpose);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}