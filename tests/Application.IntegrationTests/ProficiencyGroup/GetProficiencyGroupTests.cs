using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Proficiency.CreateProficiency;
using Educar.Backend.Application.Commands.ProficiencyGroup.CreateProficiencyGroup;
using Educar.Backend.Application.Queries.ProficiencyGroup;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.ProficiencyGroup;

[TestFixture]
public class GetProficiencyGroupTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidId_ShouldReturnProficiencyGroup()
    {
        // Arrange
        var createProficiencyCommand = new CreateProficiencyCommand("Test Proficiency", "Description", "Purpose");
        var createdProficiencyResponse = await SendAsync(createProficiencyCommand);

        var createGroupCommand = new CreateProficiencyGroupCommand("Test Group", "Description")
        {
            ProficiencyIds = new List<Guid> { createdProficiencyResponse.Id }
        };
        var createdGroupResponse = await SendAsync(createGroupCommand);
        var groupId = createdGroupResponse.Id;

        var query = new GetProficiencyGroupQuery { Id = groupId };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Test Group"));
        Assert.That(result.Description, Is.EqualTo("Description"));
        Assert.That(result.Proficiencies, Is.Not.Empty);
        Assert.That(result.Proficiencies.First().Name, Is.EqualTo("Test Proficiency"));
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        var query = new GetProficiencyGroupQuery { Id = Guid.NewGuid() };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenPageAndPageSize_ShouldReturnPaginatedProficiencyGroups()
    {
        // Arrange
        var createProficiencyCommand = new CreateProficiencyCommand("Test Proficiency", "Description", "Purpose");
        var createdProficiencyResponse = await SendAsync(createProficiencyCommand);

        for (var i = 1; i <= 20; i++)
        {
            var command = new CreateProficiencyGroupCommand($"Test Group {i}", "Description")
            {
                ProficiencyIds = new List<Guid> { createdProficiencyResponse.Id }
            };
            await SendAsync(command);
        }

        var query = new GetProficiencyGroupsPaginatedQuery { PageNumber = 1, PageSize = 10 };

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
        var createProficiencyCommand = new CreateProficiencyCommand("Test Proficiency", "Description", "Purpose");
        var createdProficiencyResponse = await SendAsync(createProficiencyCommand);

        for (var i = 1; i <= 2; i++)
        {
            var command = new CreateProficiencyGroupCommand($"Test Group {i}", "Description")
            {
                ProficiencyIds = new List<Guid> { createdProficiencyResponse.Id }
            };
            await SendAsync(command);
        }

        var query = new GetProficiencyGroupsPaginatedQuery { PageNumber = 2, PageSize = 1 };

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
        var createProficiencyCommand = new CreateProficiencyCommand("Test Proficiency", "Description", "Purpose");
        var createdProficiencyResponse = await SendAsync(createProficiencyCommand);

        for (var i = 1; i <= 20; i++)
        {
            var command = new CreateProficiencyGroupCommand($"Test Group {i}", "Description")
            {
                ProficiencyIds = new List<Guid> { createdProficiencyResponse.Id }
            };
            await SendAsync(command);
        }

        var query = new GetProficiencyGroupsPaginatedQuery { PageNumber = 3, PageSize = 10 };

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