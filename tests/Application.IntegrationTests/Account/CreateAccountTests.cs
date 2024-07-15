using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Account.CreateAccount;
using Educar.Backend.Application.Commands.School.CreateSchool;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
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
            Role: UserRole.Admin
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
            Assert.That(createdAccount.Role, Is.EqualTo(UserRole.Admin));
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
    public async Task ShouldThrowNotFoundException_WhenClientIdIsInvalid()
    {
        var schoolCommand = new CreateSchoolCommand("school", _client.Id);
        var schoolResponse = await SendAsync(schoolCommand);

        var command = new CreateAccountCommand(
            Name: "New Account",
            Email: "new.account@example.com",
            RegistrationNumber: "123456",
            ClientId: Guid.NewGuid(), // Invalid ClientId
            Role: UserRole.Student
        )
        {
            SchoolId = schoolResponse.Id
        };

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
        var schoolCommand = new CreateSchoolCommand("school", _client.Id);
        var schoolResponse = await SendAsync(schoolCommand);

        var oldAccountCommand = new CreateAccountCommand(
            Name: "Existing Account",
            Email: "existing.account@example.com",
            RegistrationNumber: "123456",
            ClientId: _client.Id,
            Role: UserRole.Student)
        {
            AverageScore = 100.50m,
            EventAverageScore = 95.75m,
            Stars = 4,
            SchoolId = schoolResponse.Id
        };

        // Act
        await SendAsync(oldAccountCommand);

        var command = new CreateAccountCommand(
            Name: "New Account",
            Email: "existing.account@example.com", // Duplicate email
            RegistrationNumber: "654321",
            ClientId: _client.Id,
            Role: UserRole.Student
        )
        {
            SchoolId = schoolResponse.Id
        };

        // Act & Assert
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenSchoolIdIsRequiredAndMissing()
    {
        var command = new CreateAccountCommand(
            Name: "New Account",
            Email: "new.account@example.com",
            RegistrationNumber: "123456",
            ClientId: _client.Id,
            Role: UserRole.Student // Non-admin role
        );

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task ShouldNotThrowValidationException_WhenSchoolIdIsRequiredAndPresent()
    {
        var schoolCommand = new CreateSchoolCommand("school", _client.Id);
        var schoolResponse = await SendAsync(schoolCommand);

        var command = new CreateAccountCommand(
            Name: "New Account",
            Email: "new.account@example.com",
            RegistrationNumber: "123456",
            ClientId: _client.Id,
            Role: UserRole.Student // Non-admin role
        )
        {
            SchoolId = schoolResponse.Id
        };

        var response = await SendAsync(command);

        var createdAccount =
            await Context.Accounts.Include(a => a.School).FirstOrDefaultAsync(a => a.Id == response.Id);
        Assert.That(createdAccount, Is.Not.Null);
        Assert.That(createdAccount.School, Is.Not.Null);
        Assert.That(createdAccount.School.Id, Is.EqualTo(schoolResponse.Id));
    }

    [Test]
    public void ShouldNotThrowValidationException_WhenSchoolIdIsNotRequired()
    {
        var command = new CreateAccountCommand(
            Name: "New Admin Account",
            Email: "new.admin@example.com",
            RegistrationNumber: "123456",
            ClientId: _client.Id,
            Role: UserRole.Admin // Admin role
        );

        Assert.DoesNotThrowAsync(async () => await SendAsync(command));
    }
}