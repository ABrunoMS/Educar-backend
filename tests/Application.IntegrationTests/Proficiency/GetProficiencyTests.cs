using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Proficiency.CreateProficiency;
using Educar.Backend.Application.Commands.ProficiencyGroup.CreateProficiencyGroup;
using Educar.Backend.Application.Queries.Proficiency;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Proficiency;

[TestFixture]
public class GetProficiencyTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidId_ShouldReturnProficiency()
    {
        // Arrange
        var createCommand = new CreateProficiencyCommand("Test Proficiency", "Description", "Purpose");
        var createdResponse = await SendAsync(createCommand);
        var proficiencyId = createdResponse.Id;

        var createproficiencyGroupCommand = new CreateProficiencyGroupCommand("Test Proficiency Group", "description")
        {
            ProficiencyIds = new List<Guid> { proficiencyId }
        };
        var createdPGResponseDto = await SendAsync(createproficiencyGroupCommand);

        var query = new GetProficiencyQuery { Id = proficiencyId };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Name, Is.EqualTo("Test Proficiency"));
            Assert.That(result.Description, Is.EqualTo("Description"));
            Assert.That(result.Purpose, Is.EqualTo("Purpose"));
            Assert.That(result.ProficiencyGroups, Is.Not.Empty);
        });
        Assert.That(result.ProficiencyGroups.First().Id, Is.EqualTo(createdPGResponseDto.Id));
        // Assert that the proficiency groups are correctly included
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        var query = new GetProficiencyQuery { Id = Guid.NewGuid() };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenPageAndPageSize_ShouldReturnPaginatedProficiencies()
    {
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var command = new CreateProficiencyCommand($"Test Proficiency {i}", "Description", "Purpose");
            await SendAsync(command);
        }

        var query = new GetProficienciesPaginatedQuery { PageNumber = 1, PageSize = 10 };

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
        for (var i = 1; i <= 2; i++)
        {
            var command = new CreateProficiencyCommand($"Test Proficiency {i}", "Description", "Purpose");
            await SendAsync(command);
        }

        var query = new GetProficienciesPaginatedQuery { PageNumber = 2, PageSize = 1 };

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
        for (var i = 1; i <= 20; i++)
        {
            var command = new CreateProficiencyCommand($"Test Proficiency {i}", "Description", "Purpose");
            await SendAsync(command);
        }

        var query = new GetProficienciesPaginatedQuery { PageNumber = 3, PageSize = 10 };

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