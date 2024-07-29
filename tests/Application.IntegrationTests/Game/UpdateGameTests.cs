using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Game.CreateGame;
using Educar.Backend.Application.Commands.Game.UpdateGame;
using Educar.Backend.Application.Commands.ProficiencyGroup.CreateProficiencyGroup;
using Educar.Backend.Application.Commands.Subject.CreateSubject;
using Educar.Backend.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
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
        var createSubjectCommand = new CreateSubjectCommand("Original Subject", "Original Subject Description");
        var createdSubjectResponse = await SendAsync(createSubjectCommand);

        var createProficiencyGroupCommand =
            new CreateProficiencyGroupCommand("Original ProficiencyGroup", "Original ProficiencyGroup Description");
        var createdProficiencyGroupResponse = await SendAsync(createProficiencyGroupCommand);

        var createCommand =
            new CreateGameCommand("Original Name", "Original Description", "Original Lore", "Original Purpose")
            {
                SubjectIds = new List<Guid> { createdSubjectResponse.Id },
                ProficiencyGroupIds = new List<Guid> { createdProficiencyGroupResponse.Id }
            };
        var createdResponse = await SendAsync(createCommand);
        var gameId = createdResponse.Id;

        var updateSubjectCommand = new CreateSubjectCommand("Updated Subject", "Updated Subject Description");
        var updatedSubjectResponse = await SendAsync(updateSubjectCommand);

        var updateProficiencyGroupCommand =
            new CreateProficiencyGroupCommand("Updated ProficiencyGroup", "Updated ProficiencyGroup Description");
        var updatedProficiencyGroupResponse = await SendAsync(updateProficiencyGroupCommand);

        var updateCommand = new UpdateGameCommand
        {
            Id = gameId,
            Name = "Updated Name",
            Description = "Updated Description",
            Lore = "Updated Lore",
            Purpose = "Updated Purpose",
            SubjectIds = new List<Guid> { updatedSubjectResponse.Id },
            ProficiencyGroupIds = new List<Guid> { updatedProficiencyGroupResponse.Id }
        };

        // Act
        await SendAsync(updateCommand);

        // Assert
        if (Context.Games != null)
        {
            var updatedGame = await Context.Games
                .Include(g => g.GameSubjects)
                .ThenInclude(gs => gs.Subject)
                .Include(g => g.GameProficiencyGroups)
                .ThenInclude(gpg => gpg.ProficiencyGroup)
                .FirstOrDefaultAsync(g => g.Id == gameId);
            Assert.That(updatedGame, Is.Not.Null);
            Assert.That(updatedGame.Name, Is.EqualTo("Updated Name"));
            Assert.That(updatedGame.Description, Is.EqualTo("Updated Description"));
            Assert.That(updatedGame.Lore, Is.EqualTo("Updated Lore"));
            Assert.That(updatedGame.Purpose, Is.EqualTo("Updated Purpose"));
            Assert.That(updatedGame.GameSubjects, Has.Count.EqualTo(1));
            Assert.That(updatedGame.GameSubjects.First().Subject.Name, Is.EqualTo("Updated Subject"));
            Assert.That(updatedGame.GameProficiencyGroups, Has.Count.EqualTo(1));
            Assert.That(updatedGame.GameProficiencyGroups.First().ProficiencyGroup.Name,
                Is.EqualTo("Updated ProficiencyGroup"));
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