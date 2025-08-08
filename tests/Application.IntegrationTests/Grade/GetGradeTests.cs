using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Grade.CreateGradeCommand;
using Educar.Backend.Application.Queries.Grade;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Grade;

[TestFixture]
public class GetGradeTests : TestBase
{
    private const string GradeName = "Test Grade";
    private const string GradeDescription = "Test Description";

    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldReturnGrade()
    {
        // Arrange
        var createCommand = new CreateGradeCommand(GradeName, GradeDescription);
        var createResponse = await SendAsync(createCommand);

        var query = new GetGradeQuery { Id = createResponse.Id };

        // Act
        var gradeResponse = await SendAsync(query);

        // Assert
        Assert.That(gradeResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(gradeResponse.Id, Is.EqualTo(createResponse.Id));
            Assert.That(gradeResponse.Name, Is.EqualTo(GradeName));
            Assert.That(gradeResponse.Description, Is.EqualTo(GradeDescription));
        });
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetGradeQuery { Id = Guid.NewGuid() };

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenValidPaginationRequest_ShouldReturnPaginatedGrades()
    {
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var command = new CreateGradeCommand($"Test Grade {i}", GradeDescription);
            await SendAsync(command);
        }

        var query = new GetGradesPaginatedQuery { PageNumber = 1, PageSize = 10 };

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
    public async Task GivenSpecificPageRequest_ShouldReturnCorrectPage()
    {
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var command = new CreateGradeCommand($"Test Grade {i}", GradeDescription);
            await SendAsync(command);
        }

        var query = new GetGradesPaginatedQuery { PageNumber = 2, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(10));
            Assert.That(result.PageNumber, Is.EqualTo(2));
            Assert.That(result.TotalCount, Is.EqualTo(20));
            Assert.That(result.TotalPages, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GivenOutOfRangePageRequest_ShouldReturnEmptyPage()
    {
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var command = new CreateGradeCommand($"Test Grade {i}", GradeDescription);
            await SendAsync(command);
        }

        var query = new GetGradesPaginatedQuery { PageNumber = 3, PageSize = 10 };

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