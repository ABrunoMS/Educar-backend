using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Account.CreateAccount;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Account;

[TestFixture]
public class CreateAccountTests : TestBase
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

    [Test]
    public async Task GivenValidRequest_ShouldCreateAccount()
    {
        // Arrange

        var command = new CreateAccountCommand(
            Name: "New Account",
            Email: "new.account@example.com",
            RegistrationNumber: "123456",
            ClientId: _client.Id,
            Role: UserRole.Student
        )
        {
            AverageScore = 100.50m,
            EventAverageScore = 95.75m,
            Stars = 4
        };

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<CreatedResponseDto>());

        if (Context.Accounts != null)
        {
            var createdAccount = await Context.Accounts.FindAsync(response.Id);
            Assert.That(createdAccount, Is.Not.Null);
            Assert.That(createdAccount.Name, Is.EqualTo("New Account"));
            Assert.That(createdAccount.Email, Is.EqualTo("new.account@example.com"));
            Assert.That(createdAccount.RegistrationNumber, Is.EqualTo("123456"));
            Assert.That(createdAccount.AverageScore, Is.EqualTo(100.50m));
            Assert.That(createdAccount.EventAverageScore, Is.EqualTo(95.75m));
            Assert.That(createdAccount.Stars, Is.EqualTo(4));
            Assert.That(createdAccount.ClientId, Is.EqualTo(_client.Id));
            Assert.That(createdAccount.Role, Is.EqualTo(UserRole.Student));
        }
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new CreateAccountCommand(
            Name: string.Empty,
            Email: "new.account@example.com",
            RegistrationNumber: "123456",
            ClientId: Guid.NewGuid(),
            Role: UserRole.Student
        );

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenEmailIsInvalid()
    {
        var command = new CreateAccountCommand(
            Name: "New Account",
            Email: "invalid-email",
            RegistrationNumber: "123456",
            ClientId: Guid.NewGuid(),
            Role: UserRole.Student
        );

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenRegistrationNumberIsEmpty()
    {
        var command = new CreateAccountCommand(
            Name: "New Account",
            Email: "new.account@example.com",
            RegistrationNumber: string.Empty,
            ClientId: Guid.NewGuid(),
            Role: UserRole.Student
        );

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenClientIdIsInvalid()
    {
        var command = new CreateAccountCommand(
            Name: "New Account",
            Email: "new.account@example.com",
            RegistrationNumber: "123456",
            ClientId: Guid.NewGuid(), // Invalid ClientId
            Role: UserRole.Student
        );

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenRoleIsInvalid()
    {
        var command = new CreateAccountCommand(
            Name: "New Account",
            Email: "new.account@example.com",
            RegistrationNumber: "123456",
            ClientId: Guid.NewGuid(),
            Role: (UserRole)999 // Invalid Role
        );

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenEmailIsNotUnique()
    {
        var oldAccountCommand = new CreateAccountCommand(
            Name: "Existing Account",
            Email: "existing.account@example.com",
            RegistrationNumber: "123456",
            ClientId: _client.Id,
            Role: UserRole.Student
        )
        {
            AverageScore = 100.50m,
            EventAverageScore = 95.75m,
            Stars = 4
        };

        // Act
        await SendAsync(oldAccountCommand);

        var command = new CreateAccountCommand(
            Name: "New Account",
            Email: "existing.account@example.com", // Duplicate email
            RegistrationNumber: "654321",
            ClientId: _client.Id,
            Role: UserRole.Student
        );

        // Act & Assert
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}