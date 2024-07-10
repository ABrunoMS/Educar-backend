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

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _client = new Domain.Entities.Client("Existing Client");
        Context.Clients.Add(_client);
        Context.SaveChanges();

        _account = new Domain.Entities.Account("Existing Account", "existing.account@example.com", "123456",
            UserRole.Student)
        {
            ClientId = _client.Id,
            Role = UserRole.Student,
            AverageScore = 100.50m,
            EventAverageScore = 95.75m,
            Stars = 4
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
}