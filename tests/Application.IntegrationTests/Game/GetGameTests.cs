using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Game.CreateGame;
using Educar.Backend.Application.Commands.ProficiencyGroup.CreateProficiencyGroup;
using Educar.Backend.Application.Commands.Subject.CreateSubject;
using Educar.Backend.Application.Queries.Game;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Game;

[TestFixture]
public class GetGameTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidId_ShouldReturnGame()
    {
        // Arrange
        var createSubjectCommand = new CreateSubjectCommand("Test Subject", "Subject Description");
        var createdSubjectResponse = await SendAsync(createSubjectCommand);

        var createProficiencyGroupCommand =
            new CreateProficiencyGroupCommand("Test ProficiencyGroup", "ProficiencyGroup Description");
        var createdProficiencyGroupResponse = await SendAsync(createProficiencyGroupCommand);

        var createCommand = new CreateGameCommand("Test Game", "Description", "Lore", "Purpose")
        {
            SubjectIds = new List<Guid> { createdSubjectResponse.Id },
            ProficiencyGroupIds = new List<Guid> { createdProficiencyGroupResponse.Id }
        };
        var createdResponse = await SendAsync(createCommand);
        var gameId = createdResponse.Id;

        var query = new GetGameQuery { Id = gameId };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Test Game"));
        Assert.That(result.Description, Is.EqualTo("Description"));
        Assert.That(result.Lore, Is.EqualTo("Lore"));
        Assert.That(result.Purpose, Is.EqualTo("Purpose"));
        Assert.That(result.Subjects, Is.Not.Empty);
        Assert.That(result.Subjects.First().Name, Is.EqualTo("Test Subject"));
        Assert.That(result.ProficiencyGroups, Is.Not.Empty);
        Assert.That(result.ProficiencyGroups.First().Name, Is.EqualTo("Test ProficiencyGroup"));
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        var query = new GetGameQuery { Id = Guid.NewGuid() };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenPageAndPageSize_ShouldReturnPaginatedGames()
    {
        // Arrange
        var createSubjectCommand = new CreateSubjectCommand("Test Subject", "Subject Description");
        var createdSubjectResponse = await SendAsync(createSubjectCommand);

        var createProficiencyGroupCommand =
            new CreateProficiencyGroupCommand("Test ProficiencyGroup", "ProficiencyGroup Description");
        var createdProficiencyGroupResponse = await SendAsync(createProficiencyGroupCommand);

        for (var i = 1; i <= 20; i++)
        {
            var command = new CreateGameCommand($"Test Game {i}", "Description", "Lore", "Purpose")
            {
                SubjectIds = new List<Guid> { createdSubjectResponse.Id },
                ProficiencyGroupIds = new List<Guid> { createdProficiencyGroupResponse.Id }
            };
            await SendAsync(command);
        }

        var query = new GetGamesPaginatedQuery { PageNumber = 1, PageSize = 10 };

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
        var createSubjectCommand = new CreateSubjectCommand("Test Subject", "Subject Description");
        var createdSubjectResponse = await SendAsync(createSubjectCommand);

        var createProficiencyGroupCommand =
            new CreateProficiencyGroupCommand("Test ProficiencyGroup", "ProficiencyGroup Description");
        var createdProficiencyGroupResponse = await SendAsync(createProficiencyGroupCommand);

        for (var i = 1; i <= 2; i++)
        {
            var command = new CreateGameCommand($"Test Game {i}", "Description", "Lore", "Purpose")
            {
                SubjectIds = new List<Guid> { createdSubjectResponse.Id },
                ProficiencyGroupIds = new List<Guid> { createdProficiencyGroupResponse.Id }
            };
            await SendAsync(command);
        }

        var query = new GetGamesPaginatedQuery { PageNumber = 2, PageSize = 1 };

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
        Assert.That(result.Items, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GivenPageAndPageSize_ShouldReturnEmptyWhenOutOfRange()
    {
        // Arrange
        var createSubjectCommand = new CreateSubjectCommand("Test Subject", "Subject Description");
        var createdSubjectResponse = await SendAsync(createSubjectCommand);

        var createProficiencyGroupCommand =
            new CreateProficiencyGroupCommand("Test ProficiencyGroup", "ProficiencyGroup Description");
        var createdProficiencyGroupResponse = await SendAsync(createProficiencyGroupCommand);

        for (var i = 1; i <= 20; i++)
        {
            var command = new CreateGameCommand($"Test Game {i}", "Description", "Lore", "Purpose")
            {
                SubjectIds = new List<Guid> { createdSubjectResponse.Id },
                ProficiencyGroupIds = new List<Guid> { createdProficiencyGroupResponse.Id }
            };
            await SendAsync(command);
        }

        var query = new GetGamesPaginatedQuery { PageNumber = 3, PageSize = 10 };

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