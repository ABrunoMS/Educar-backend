using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Subject.CreateSubject;
using Educar.Backend.Application.Queries.Subject;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Subject;

[TestFixture]
public class GetSubjectTests : TestBase
{
    private const string SubjectName = "Test Subject";
    private const string SubjectDescription = "Test Description";

    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldReturnSubject()
    {
        // Arrange
        var createCommand = new CreateSubjectCommand(SubjectName, SubjectDescription);
        var createResponse = await SendAsync(createCommand);

        var query = new GetSubjectQuery { Id = createResponse.Id };

        // Act
        var subjectResponse = await SendAsync(query);

        // Assert
        Assert.That(subjectResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(subjectResponse.Id, Is.EqualTo(createResponse.Id));
            Assert.That(subjectResponse.Name, Is.EqualTo(SubjectName));
            Assert.That(subjectResponse.Description, Is.EqualTo(SubjectDescription));
        });
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetSubjectQuery { Id = Guid.NewGuid() };

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenValidPaginationRequest_ShouldReturnPaginatedSubjects()
    {
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var command = new CreateSubjectCommand($"Test Subject {i}", SubjectDescription);
            await SendAsync(command);
        }

        var query = new GetSubjectsPaginatedQuery { PageNumber = 1, PageSize = 10 };

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
            var command = new CreateSubjectCommand($"Test Subject {i}", SubjectDescription);
            await SendAsync(command);
        }

        var query = new GetSubjectsPaginatedQuery { PageNumber = 2, PageSize = 10 };

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
            var command = new CreateSubjectCommand($"Test Subject {i}", SubjectDescription);
            await SendAsync(command);
        }

        var query = new GetSubjectsPaginatedQuery { PageNumber = 3, PageSize = 10 };

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