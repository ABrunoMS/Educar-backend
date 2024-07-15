using Ardalis.GuardClauses;
using Educar.Backend.Application.Queries.Account;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;

namespace Educar.Backend.Application.IntegrationTests.Account;

using static Testing;

[TestFixture]
public class GetAccountTests : TestBase
{
    private Domain.Entities.Client _client;
    private Domain.Entities.Account _account;
    private Domain.Entities.School _school;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _client = new Domain.Entities.Client("Existing Client");
        Context.Clients.Add(_client);
        Context.SaveChanges();

        _school = new Domain.Entities.School("Existing School")
        {
            Description = "A test school",
            Address = new Domain.Entities.Address("123 Main St", "Test City", "Test State", "12345", "Test Country")
            {
                Lat = 40.712776m,
                Lng = -74.005974m
            },
            Client = _client
        };
        Context.Schools.Add(_school);
        Context.SaveChanges();

        _account = new Domain.Entities.Account("Existing Account", "existing.account@example.com", "123456",
            UserRole.Student)
        {
            ClientId = _client.Id,
            Role = UserRole.Student,
            AverageScore = 100.50m,
            EventAverageScore = 95.75m,
            Stars = 4,
            SchoolId = _school.Id
        };
        Context.Accounts.Add(_account);
        Context.SaveChanges();
    }

    [Test]
    public async Task GivenValidId_ShouldReturnAccount()
    {
        // Arrange
        var query = new GetAccountQuery { Id = _account.Id };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(_account.Id));
        Assert.That(result.Name, Is.EqualTo("Existing Account"));
        Assert.That(result.Email, Is.EqualTo("existing.account@example.com"));
        Assert.That(result.RegistrationNumber, Is.EqualTo("123456"));
        Assert.That(result.AverageScore, Is.EqualTo(100.50m));
        Assert.That(result.EventAverageScore, Is.EqualTo(95.75m));
        Assert.That(result.Stars, Is.EqualTo(4));
        Assert.That(result.ClientId, Is.EqualTo(_client.Id));
        Assert.That(result.Role, Is.EqualTo(UserRole.Student));
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetAccountQuery { Id = Guid.NewGuid() }; // Invalid Id

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenNoAccounts_ShouldReturnNull()
    {
        // Arrange
        Context.Accounts.Remove(_account);
        await Context.SaveChangesAsync();

        var query = new GetAccountQuery { Id = _account.Id };

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenValidPaginationRequest_ShouldReturnPaginatedAccounts()
    {
        // Arrange
        for (int i = 1; i <= 20; i++)
        {
            var account = new Domain.Entities.Account($"Test Account {i}", $"test.account{i}@example.com", "123456",
                UserRole.Student)
            {
                ClientId = _client.Id,
                Role = UserRole.Student,
                AverageScore = 100.50m,
                EventAverageScore = 95.75m,
                Stars = 4
            };
            Context.Accounts.Add(account);
        }

        await Context.SaveChangesAsync();

        var query = new GetAccountsPaginatedQuery { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(10));
            Assert.That(result.PageNumber, Is.EqualTo(1));
            Assert.That(result.TotalCount, Is.EqualTo(21)); // Including the initial account
            Assert.That(result.TotalPages, Is.EqualTo(3));
        });
    }

    [Test]
    public async Task GivenSpecificPageRequest_ShouldReturnCorrectPage()
    {
        // Arrange
        for (int i = 1; i <= 20; i++)
        {
            var account = new Domain.Entities.Account($"Test Account {i}", $"test.account{i}@example.com", "123456",
                UserRole.Student)
            {
                ClientId = _client.Id,
                Role = UserRole.Student,
                AverageScore = 100.50m,
                EventAverageScore = 95.75m,
                Stars = 4
            };
            Context.Accounts.Add(account);
        }

        await Context.SaveChangesAsync();

        var query = new GetAccountsPaginatedQuery { PageNumber = 2, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items.Count, Is.EqualTo(10));
        Assert.That(result.PageNumber, Is.EqualTo(2));
        Assert.That(result.TotalCount, Is.EqualTo(21)); // Including the initial account
        Assert.That(result.TotalPages, Is.EqualTo(3));
    }

    [Test]
    public async Task GivenOutOfRangePageRequest_ShouldReturnEmptyPage()
    {
        // Arrange
        for (int i = 1; i <= 20; i++)
        {
            var account = new Domain.Entities.Account($"Test Account {i}", $"test.account{i}@example.com", "123456",
                UserRole.Student)
            {
                ClientId = _client.Id,
                Role = UserRole.Student,
                AverageScore = 100.50m,
                EventAverageScore = 95.75m,
                Stars = 4
            };
            Context.Accounts.Add(account);
        }

        await Context.SaveChangesAsync();

        var query = new GetAccountsPaginatedQuery { PageNumber = 3, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items.Count, Is.EqualTo(1)); // Only one account on the third page
        Assert.That(result.PageNumber, Is.EqualTo(3));
        Assert.That(result.TotalCount, Is.EqualTo(21)); // Including the initial account
        Assert.That(result.TotalPages, Is.EqualTo(3));
    }

    [Test]
    public async Task GivenValidPaginationRequest_ShouldReturnPaginatedAccountsBySchool()
    {
        // Arrange
        for (int i = 1; i <= 20; i++)
        {
            var account = new Domain.Entities.Account($"Test Account {i}", $"test.account{i}@example.com", "123456",
                UserRole.Student)
            {
                ClientId = _client.Id,
                Role = UserRole.Student,
                AverageScore = 100.50m,
                EventAverageScore = 95.75m,
                Stars = 4,
                SchoolId = _school.Id
            };
            Context.Accounts.Add(account);
        }

        await Context.SaveChangesAsync();

        var query = new GetAccountsBySchoolPaginatedQuery(_school.Id) { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(10));
            Assert.That(result.PageNumber, Is.EqualTo(1));
            Assert.That(result.TotalCount, Is.EqualTo(21)); // Including the initial account
            Assert.That(result.TotalPages, Is.EqualTo(3));
        });
    }

    [Test]
    public async Task GivenSpecificPageRequestBySchool_ShouldReturnCorrectPage()
    {
        // Arrange
        for (int i = 1; i <= 20; i++)
        {
            var account = new Domain.Entities.Account($"Test Account {i}", $"test.account{i}@example.com", "123456",
                UserRole.Student)
            {
                ClientId = _client.Id,
                Role = UserRole.Student,
                AverageScore = 100.50m,
                EventAverageScore = 95.75m,
                Stars = 4,
                SchoolId = _school.Id
            };
            Context.Accounts.Add(account);
        }

        await Context.SaveChangesAsync();

        var query = new GetAccountsBySchoolPaginatedQuery(_school.Id) { PageNumber = 2, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items.Count, Is.EqualTo(10));
        Assert.That(result.PageNumber, Is.EqualTo(2));
        Assert.That(result.TotalCount, Is.EqualTo(21)); // Including the initial account
        Assert.That(result.TotalPages, Is.EqualTo(3));
    }

    [Test]
    public async Task GivenOutOfRangePageRequestBySchool_ShouldReturnEmptyPage()
    {
        // Arrange
        for (int i = 1; i <= 20; i++)
        {
            var account = new Domain.Entities.Account($"Test Account {i}", $"test.account{i}@example.com", "123456",
                UserRole.Student)
            {
                ClientId = _client.Id,
                Role = UserRole.Student,
                AverageScore = 100.50m,
                EventAverageScore = 95.75m,
                Stars = 4,
                SchoolId = _school.Id
            };
            Context.Accounts.Add(account);
        }

        await Context.SaveChangesAsync();

        var query = new GetAccountsBySchoolPaginatedQuery(_school.Id) { PageNumber = 3, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items.Count, Is.EqualTo(1)); // Only one account on the third page
        Assert.That(result.PageNumber, Is.EqualTo(3));
        Assert.That(result.TotalCount, Is.EqualTo(21)); // Including the initial account
        Assert.That(result.TotalPages, Is.EqualTo(3));
    }
}