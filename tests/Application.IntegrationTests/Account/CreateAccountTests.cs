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
    private Domain.Entities.School _school;
    private Domain.Entities.Class _class1;
    private Domain.Entities.Class _class2;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _client = new Domain.Entities.Client("Existing Client");
        Context.Clients.Add(_client);

        _school = new Domain.Entities.School("Test School");
        Context.Schools.Add(_school);

        _class1 = new Domain.Entities.Class("Class 1", "Description 1", ClassPurpose.Default)
        {
            School = _school
        };
        _class2 = new Domain.Entities.Class("Class 2", "Description 2", ClassPurpose.Reinforcement)
        {
            School = _school
        };
        Context.Classes.AddRange(_class1, _class2);

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
            Stars = 4,
            SchoolId = _school.Id,
            ClassIds = new List<Guid> { _class1.Id, _class2.Id }
        };

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<CreatedResponseDto>());

        var createdAccount = await Context.Accounts
            .Include(a => a.School)
            .Include(a => a.AccountClasses).ThenInclude(ac => ac.Class)
            .FirstOrDefaultAsync(a => a.Id == response.Id);

        Assert.That(createdAccount, Is.Not.Null);
        Assert.That(createdAccount.Name, Is.EqualTo("New Account"));
        Assert.That(createdAccount.Email, Is.EqualTo("new.account@example.com"));
        Assert.That(createdAccount.RegistrationNumber, Is.EqualTo("123456"));
        Assert.That(createdAccount.AverageScore, Is.EqualTo(100.50m));
        Assert.That(createdAccount.EventAverageScore, Is.EqualTo(95.75m));
        Assert.That(createdAccount.Stars, Is.EqualTo(4));
        Assert.That(createdAccount.ClientId, Is.EqualTo(_client.Id));
        Assert.That(createdAccount.Role, Is.EqualTo(UserRole.Admin));
        Assert.That(createdAccount.School, Is.Not.Null);
        Assert.That(createdAccount.School.Id, Is.EqualTo(_school.Id));
        Assert.That(createdAccount.AccountClasses.Count, Is.EqualTo(2));
        Assert.That(createdAccount.AccountClasses.Select(ac => ac.ClassId).ToList(),
            Is.EquivalentTo(new List<Guid> { _class1.Id, _class2.Id }));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new CreateAccountCommand(
            Name: string.Empty,
            Email: "new.account@example.com",
            RegistrationNumber: "123456",
            ClientId: _client.Id,
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
            ClientId: _client.Id,
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
            ClientId: _client.Id,
            Role: UserRole.Student
        );

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowNotFoundException_WhenClientIdIsInvalid()
    {
        var command = new CreateAccountCommand(
            Name: "New Account",
            Email: "new.account@example.com",
            RegistrationNumber: "123456",
            ClientId: Guid.NewGuid(), // Invalid ClientId
            Role: UserRole.Admin
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
            ClientId: _client.Id,
            Role: (UserRole)999 // Invalid Role
        );

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenEmailIsNotUnique()
    {
        var existingCommand = new CreateAccountCommand(
            Name: "Existing Account",
            Email: "existing.account@example.com",
            RegistrationNumber: "123456",
            ClientId: _client.Id,
            Role: UserRole.Admin
        )
        {
            AverageScore = 100.50m,
            EventAverageScore = 95.75m,
            Stars = 4,
            SchoolId = _school.Id
        };

        await SendAsync(existingCommand);

        var command = new CreateAccountCommand(
            Name: "New Account",
            Email: "existing.account@example.com", // Duplicate email
            RegistrationNumber: "654321",
            ClientId: _client.Id,
            Role: UserRole.Student
        )
        {
            SchoolId = _school.Id
        };

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
        var command = new CreateAccountCommand(
            Name: "New Account",
            Email: "new.account@example.com",
            RegistrationNumber: "123456",
            ClientId: _client.Id,
            Role: UserRole.Student // Non-admin role
        )
        {
            SchoolId = _school.Id,
            ClassIds = new List<Guid> { _class1.Id, _class2.Id }
        };

        var response = await SendAsync(command);

        var createdAccount = await Context.Accounts
            .Include(a => a.School)
            .Include(a => a.AccountClasses).ThenInclude(ac => ac.Class)
            .FirstOrDefaultAsync(a => a.Id == response.Id);

        Assert.That(createdAccount, Is.Not.Null);
        Assert.That(createdAccount.School, Is.Not.Null);
        Assert.That(createdAccount.School.Id, Is.EqualTo(_school.Id));
        Assert.That(createdAccount.AccountClasses, Has.Count.EqualTo(2));
        Assert.That(createdAccount.AccountClasses.Select(ac => ac.ClassId).ToList(),
            Is.EquivalentTo(new List<Guid> { _class1.Id, _class2.Id }));
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

    [Test]
    public void ShouldThrowValidationException_WhenOneOrMoreClassIdsAreInvalid()
    {
        var command = new CreateAccountCommand(
            Name: "New Account",
            Email: "new.account@example.com",
            RegistrationNumber: "123456",
            ClientId: _client.Id,
            Role: UserRole.Student
        )
        {
            SchoolId = _school.Id,
            ClassIds = new List<Guid> { _class1.Id, Guid.NewGuid() } // One valid and one invalid ClassId
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}