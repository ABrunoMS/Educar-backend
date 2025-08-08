using Ardalis.GuardClauses;
using Educar.Backend.Application.Queries.School;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.School;

[TestFixture]
public class GetSchoolTests : TestBase
{
    private Domain.Entities.Client _client;
    private Domain.Entities.Address _address;
    private Domain.Entities.School _school;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _client = new Domain.Entities.Client("Test Client");
        Context.Clients.Add(_client);
        Context.SaveChanges();

        _address = new Domain.Entities.Address("123 Main St", "Test City", "Test State", "12345", "Test Country")
        {
            Lat = 40.712776m,
            Lng = -74.005974m
        };
        Context.Addresses.Add(_address);
        Context.SaveChanges();

        _school = new Domain.Entities.School("Test School")
        {
            Description = "A test school",
            Address = _address,
            Client = _client
        };
        Context.Schools.Add(_school);
        Context.SaveChanges();
    }

    [Test]
    public async Task GivenValidId_ShouldReturnSchool()
    {
        // Arrange
        var query = new GetSchoolQuery { Id = _school.Id };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(_school.Id));
        Assert.That(result.Name, Is.EqualTo("Test School"));
        Assert.That(result.Description, Is.EqualTo("A test school"));
        Assert.That(result.Address, Is.Not.Null);
        Assert.That(result.Address.Street, Is.EqualTo("123 Main St"));
        Assert.That(result.Address.City, Is.EqualTo("Test City"));
        Assert.That(result.Address.State, Is.EqualTo("Test State"));
        Assert.That(result.Address.PostalCode, Is.EqualTo("12345"));
        Assert.That(result.Address.Country, Is.EqualTo("Test Country"));
        Assert.That(result.Address.Lat, Is.EqualTo(40.712776m));
        Assert.That(result.Address.Lng, Is.EqualTo(-74.005974m));
        Assert.That(result.Client, Is.Not.Null);
        Assert.That(result.Client.Id, Is.EqualTo(_client.Id));
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetSchoolQuery { Id = Guid.NewGuid() };

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenNoSchool_ShouldReturnNull()
    {
        // Arrange
        Context.Schools.Remove(_school);
        await Context.SaveChangesAsync();

        var query = new GetSchoolQuery { Id = _school.Id };

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenValidPaginationRequest_ShouldReturnPaginatedSchools()
    {
        // Arrange
        for (int i = 1; i <= 20; i++)
        {
            var school = new Domain.Entities.School($"Test School {i}")
            {
                Description = $"A test school {i}",
                Address = new Domain.Entities.Address($"123 Main St {i}", "Test City", "Test State", "12345",
                    "Test Country")
                {
                    Lat = 40.712776m + i,
                    Lng = -74.005974m + i
                },
                Client = _client
            };
            Context.Schools.Add(school);
        }

        await Context.SaveChangesAsync();

        var query = new GetSchoolsPaginatedQuery { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items.Count, Is.EqualTo(10));
        Assert.That(result.PageNumber, Is.EqualTo(1));
        Assert.That(result.TotalCount, Is.EqualTo(21)); // Including the initial school
        Assert.That(result.TotalPages, Is.EqualTo(3));
    }

    [Test]
    public async Task GivenSpecificPageRequest_ShouldReturnCorrectPage()
    {
        // Arrange
        for (int i = 1; i <= 20; i++)
        {
            var school = new Domain.Entities.School($"Test School {i}")
            {
                Description = $"A test school {i}",
                Address = new Domain.Entities.Address($"123 Main St {i}", "Test City", "Test State", "12345",
                    "Test Country")
                {
                    Lat = 40.712776m + i,
                    Lng = -74.005974m + i
                },
                Client = _client
            };
            Context.Schools.Add(school);
        }

        await Context.SaveChangesAsync();

        var query = new GetSchoolsPaginatedQuery { PageNumber = 2, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(10));
            Assert.That(result.PageNumber, Is.EqualTo(2));
            Assert.That(result.TotalCount, Is.EqualTo(21)); // Including the initial school
            Assert.That(result.TotalPages, Is.EqualTo(3));
        });
    }

    [Test]
    public async Task GivenOutOfRangePageRequest_ShouldReturnEmptyPage()
    {
        // Arrange
        for (int i = 1; i <= 20; i++)
        {
            var school = new Domain.Entities.School($"Test School {i}")
            {
                Description = $"A test school {i}",
                Address = new Domain.Entities.Address($"123 Main St {i}", "Test City", "Test State", "12345",
                    "Test Country")
                {
                    Lat = 40.712776m + i,
                    Lng = -74.005974m + i
                },
                Client = _client
            };
            Context.Schools.Add(school);
        }

        await Context.SaveChangesAsync();

        var query = new GetSchoolsPaginatedQuery { PageNumber = 3, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(1)); // Only one school on the third page
            Assert.That(result.PageNumber, Is.EqualTo(3));
            Assert.That(result.TotalCount, Is.EqualTo(21)); // Including the initial school
            Assert.That(result.TotalPages, Is.EqualTo(3));
        });
    }

    [Test]
    public async Task GivenValidClientId_ShouldReturnPaginatedSchoolsByClient()
    {
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var school = new Domain.Entities.School($"Test School {i}")
            {
                Description = $"A test school {i}",
                Address = new Domain.Entities.Address($"123 Main St {i}", "Test City", "Test State", "12345",
                    "Test Country")
                {
                    Lat = 40.712776m + i,
                    Lng = -74.005974m + i
                },
                Client = _client
            };
            Context.Schools.Add(school);
        }

        await Context.SaveChangesAsync();

        var query = new GetSchoolsByClientPaginatedQuery(_client.Id) { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(10));
            Assert.That(result.PageNumber, Is.EqualTo(1));
            Assert.That(result.TotalCount, Is.EqualTo(21)); // Including the initial school
            Assert.That(result.TotalPages, Is.EqualTo(3));
        });
    }

    [Test]
    public async Task GivenSpecificPageRequestByClient_ShouldReturnCorrectPage()
    {
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var school = new Domain.Entities.School($"Test School {i}")
            {
                Description = $"A test school {i}",
                Address = new Domain.Entities.Address($"123 Main St {i}", "Test City", "Test State", "12345",
                    "Test Country")
                {
                    Lat = 40.712776m + i,
                    Lng = -74.005974m + i
                },
                Client = _client
            };
            Context.Schools.Add(school);
        }

        await Context.SaveChangesAsync();

        var query = new GetSchoolsByClientPaginatedQuery(_client.Id) { PageNumber = 2, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(10));
            Assert.That(result.PageNumber, Is.EqualTo(2));
            Assert.That(result.TotalCount, Is.EqualTo(21)); // Including the initial school
            Assert.That(result.TotalPages, Is.EqualTo(3));
        });
    }

    [Test]
    public async Task GivenOutOfRangePageRequestByClient_ShouldReturnEmptyPage()
    {
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var school = new Domain.Entities.School($"Test School {i}")
            {
                Description = $"A test school {i}",
                Address = new Domain.Entities.Address($"123 Main St {i}", "Test City", "Test State", "12345",
                    "Test Country")
                {
                    Lat = 40.712776m + i,
                    Lng = -74.005974m + i
                },
                Client = _client
            };
            Context.Schools.Add(school);
        }

        await Context.SaveChangesAsync();

        var query = new GetSchoolsByClientPaginatedQuery(_client.Id) { PageNumber = 3, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(1)); // Only one school on the third page
            Assert.That(result.PageNumber, Is.EqualTo(3));
            Assert.That(result.TotalCount, Is.EqualTo(21)); // Including the initial school
            Assert.That(result.TotalPages, Is.EqualTo(3));
        });
    }
}