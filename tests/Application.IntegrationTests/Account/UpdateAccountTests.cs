using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Account.CreateAccount;
using Educar.Backend.Application.Commands.Account.UpdateAccount;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Account;

[TestFixture]
public class UpdateAccountTests : TestBase
{
    private Domain.Entities.Client _client;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _client = new Domain.Entities.Client("Existing Client");
        Context.Clients.Add(_client);
        Context.SaveChanges();
    }

    private async Task<Guid> CreateAccount()
    {
        var command = new CreateAccountCommand("Existing Account", "existing.account@example.com", "123456",
            _client.Id, UserRole.Student)
        {
            AverageScore = 100.50m,
            EventAverageScore = 95.75m,
            Stars = 4
        };

        var response = await SendAsync(command);

        return response.Id;
    }

    [Test]
    public async Task GivenValidRequest_ShouldUpdateAccount()
    {
        const string newName = "Updated Account";
        const string newRegistrationNumber = "654321";

        var accountId = await CreateAccount();

        // Arrange
        var command = new UpdateAccountCommand
        {
            Id = accountId,
            Name = newName,
            RegistrationNumber = newRegistrationNumber,
            Role = UserRole.Teacher,
            AverageScore = 200.75m,
            EventAverageScore = 150.50m,
            Stars = 5
        };

        // Act
        await SendAsync(command);
        // Assert
        var updatedAccount = await Context.Accounts.FindAsync(command.Id);
        Assert.That(updatedAccount, Is.Not.Null);
        Assert.That(updatedAccount.Name, Is.EqualTo(newName));
        Assert.That(updatedAccount.RegistrationNumber, Is.EqualTo(newRegistrationNumber));
        Assert.That(updatedAccount.Role, Is.EqualTo(UserRole.Teacher));
        Assert.That(updatedAccount.AverageScore, Is.EqualTo(200.75m));
        Assert.That(updatedAccount.EventAverageScore, Is.EqualTo(150.50m));
        Assert.That(updatedAccount.Stars, Is.EqualTo(5));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var accountId = await CreateAccount();

        var command = new UpdateAccountCommand
        {
            Id = accountId,
            Name = string.Empty,
            RegistrationNumber = "654321",
            Role = UserRole.Teacher,
            AverageScore = 200.75m,
            EventAverageScore = 150.50m,
            Stars = 5
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenRegistrationNumberIsEmpty()
    {
        var accountId = await CreateAccount();

        var command = new UpdateAccountCommand
        {
            Id = accountId,
            Name = "Updated Account",
            RegistrationNumber = string.Empty,
            Role = UserRole.Teacher,
            AverageScore = 200.75m,
            EventAverageScore = 150.50m,
            Stars = 5
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenRoleIsInvalid()
    {
        var accountId = await CreateAccount();

        var command = new UpdateAccountCommand
        {
            Id = accountId,
            Name = "Updated Account",
            RegistrationNumber = "654321",
            Role = (UserRole)999, // Invalid Role
            AverageScore = 200.75m,
            EventAverageScore = 150.50m,
            Stars = 5
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenAverageScoreIsInvalid()
    {
        var accountId = await CreateAccount();

        var command = new UpdateAccountCommand
        {
            Id = accountId,
            Name = "Updated Account",
            RegistrationNumber = "654321",
            Role = UserRole.Teacher,
            AverageScore = 1000m, // Invalid Average Score
            EventAverageScore = 150.50m,
            Stars = 5
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenEventAverageScoreIsInvalid()
    {
        var accountId = await CreateAccount();

        var command = new UpdateAccountCommand
        {
            Id = accountId,
            Name = "Updated Account",
            RegistrationNumber = "654321",
            Role = UserRole.Teacher,
            AverageScore = 200.75m,
            EventAverageScore = 1000m, // Invalid Event Average Score
            Stars = 5
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenStarsAreInvalid()
    {
        var accountId = await CreateAccount();

        var command = new UpdateAccountCommand
        {
            Id = accountId,
            Name = "Updated Account",
            RegistrationNumber = "654321",
            Role = UserRole.Teacher,
            AverageScore = 200.75m,
            EventAverageScore = 150.50m,
            Stars = 10 // Invalid Stars
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowNotFoundException_WhenIdIsInvalid()
    {
        var command = new UpdateAccountCommand
        {
            Id = Guid.NewGuid(), // Invalid Id
            Name = "Updated Account",
            RegistrationNumber = "654321",
            Role = UserRole.Teacher,
            AverageScore = 200.75m,
            EventAverageScore = 150.50m,
            Stars = 5
        };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }
}