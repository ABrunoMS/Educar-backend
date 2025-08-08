using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Class.CreateClass;
using Educar.Backend.Application.Commands.School.CreateSchool;
using Educar.Backend.Application.Queries.Class;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Class;

[TestFixture]
public class GetClassTests
{
    private Domain.Entities.Client _client;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _client = new Domain.Entities.Client("client");
        Context.Clients.Add(_client);
        Context.SaveChanges();
    }

    [Test]
    public async Task GivenValidRequest_ShouldReturnClass()
    {
        var schoolCommand = new CreateSchoolCommand("school", _client.Id);
        var schoolResponse = await SendAsync(schoolCommand);

        const string name = "name";
        var classCommand = new CreateClassCommand(name, "description", ClassPurpose.Default, schoolResponse.Id);
        var response = await SendAsync(classCommand);

        // Arrange
        var query = new GetClassQuery { Id = response.Id };

        // Act
        var queryResponse = await SendAsync(query);

        // Assert
        Assert.That(queryResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(queryResponse.Id, Is.EqualTo(response.Id));
            Assert.That(queryResponse.Name, Is.EqualTo(name));
        });
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetClassQuery { Id = Guid.NewGuid() };

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenValidPaginationRequest_ShouldReturnPaginatedClassesBySchool()
    {
        var schoolCommand = new CreateSchoolCommand("school", _client.Id);
        var schoolResponse = await SendAsync(schoolCommand);

        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var classCommand = new CreateClassCommand($"test class {i}", "description", ClassPurpose.Default,
                schoolResponse.Id);
            await SendAsync(classCommand);
        }

        var query = new GetClassesBySchoolPaginatedQuery(schoolResponse.Id) { PageNumber = 1, PageSize = 10 };

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
    public async Task GivenSpecificPageRequest_ShouldReturnCorrectPageBySchool()
    {
        var schoolCommand = new CreateSchoolCommand("school", _client.Id);
        var schoolResponse = await SendAsync(schoolCommand);

        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var classCommand = new CreateClassCommand($"test class {i}", "description", ClassPurpose.Default,
                schoolResponse.Id);
            await SendAsync(classCommand);
        }

        var query = new GetClassesBySchoolPaginatedQuery(schoolResponse.Id) { PageNumber = 2, PageSize = 10 };

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
        var schoolCommand = new CreateSchoolCommand("school", _client.Id);
        var schoolResponse = await SendAsync(schoolCommand);

        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var classCommand = new CreateClassCommand($"test class {i}", "description", ClassPurpose.Default,
                schoolResponse.Id);
            await SendAsync(classCommand);
        }

        var query = new GetClassesBySchoolPaginatedQuery(schoolResponse.Id) { PageNumber = 3, PageSize = 10 };

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