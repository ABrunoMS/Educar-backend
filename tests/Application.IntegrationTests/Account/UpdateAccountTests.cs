using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Account.CreateAccount;
using Educar.Backend.Application.Commands.Account.UpdateAccount;
using Educar.Backend.Application.Commands.Class.CreateClass;
using Educar.Backend.Application.Commands.School.CreateSchool;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
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

    private async Task<Guid> CreateAccount(UserRole role = UserRole.Admin, Guid? SchoolId = null,
        List<Guid>? ClassIds = null)
    {
        var command = new CreateAccountCommand("Existing Account", "existing.account@example.com", "123456",
            _client.Id, role)
        {
            AverageScore = 100.50m,
            EventAverageScore = 95.75m,
            Stars = 4
        };
        if (SchoolId != null)
        {
            command.SchoolId = SchoolId;
        }

        if (ClassIds != null)
        {
            command.ClassIds = ClassIds;
        }

        var response = await SendAsync(command);

        return response.Id;
    }

    [Test]
    public async Task GivenValidRequest_ShouldUpdateAccount()
    {
        const string newName = "Updated Account";
        const string newRegistrationNumber = "654321";

        var accountId = await CreateAccount(UserRole.Admin);

        // Arrange
        var command = new UpdateAccountCommand
        {
            Id = accountId,
            Name = newName,
            RegistrationNumber = newRegistrationNumber,
            AverageScore = 200.75m,
            EventAverageScore = 150.50m,
            Stars = 5,
            ClassIds = new List<Guid> { Guid.NewGuid() } // Add test class IDs
        };

        // Act
        await SendAsync(command);

        // Assert
        var updatedAccount = await Context.Accounts.Include(a => a.AccountClasses)
            .FirstOrDefaultAsync(a => a.Id == command.Id);
        Assert.That(updatedAccount, Is.Not.Null);
        Assert.That(updatedAccount.Name, Is.EqualTo(newName));
        Assert.That(updatedAccount.RegistrationNumber, Is.EqualTo(newRegistrationNumber));
        Assert.That(updatedAccount.Role, Is.EqualTo(UserRole.Admin));
        Assert.That(updatedAccount.AverageScore, Is.EqualTo(200.75m));
        Assert.That(updatedAccount.EventAverageScore, Is.EqualTo(150.50m));
        Assert.That(updatedAccount.Stars, Is.EqualTo(5));
        Assert.That(updatedAccount.AccountClasses.Select(ac => ac.ClassId), Is.EquivalentTo(command.ClassIds));
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
            AverageScore = 200.75m,
            EventAverageScore = 150.50m,
            Stars = 5
        };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenSchoolIdIsRequiredAndMissing()
    {
        var schoolCommand = new CreateSchoolCommand("school", _client.Id);
        var schoolResponse = await SendAsync(schoolCommand);
        var classCommand = new CreateClassCommand("class", "description", ClassPurpose.Default, schoolResponse.Id);
        var classResponse = await SendAsync(classCommand);
        var accountId = await CreateAccount(UserRole.Student, schoolResponse.Id, new List<Guid> { classResponse.Id });

        var command = new UpdateAccountCommand
        {
            Id = accountId,
            Name = "Updated Account",
            RegistrationNumber = "654321",
            AverageScore = 200.75m,
            EventAverageScore = 150.50m,
            Stars = 5,
            ClassIds = new List<Guid> { classResponse.Id } // Add test class IDs
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task ShouldNotThrowValidationException_WhenSchoolIdIsRequiredAndPresent()
    {
        var schoolCommand = new CreateSchoolCommand("school", _client.Id);
        var schoolResponse = await SendAsync(schoolCommand);
        var classCommand = new CreateClassCommand("class", "description", ClassPurpose.Default, schoolResponse.Id);
        var classResponse = await SendAsync(classCommand);
        var accountId = await CreateAccount(UserRole.Student, schoolResponse.Id, new List<Guid> { classResponse.Id });

        var command = new UpdateAccountCommand
        {
            Id = accountId,
            Name = "Updated Account",
            RegistrationNumber = "654321",
            AverageScore = 200.75m,
            EventAverageScore = 150.50m,
            Stars = 5,
            SchoolId = schoolResponse.Id,
            ClassIds = new List<Guid> { classResponse.Id } // Add test class IDs
        };

        await SendAsync(command);

        var updatedAccount = await Context.Accounts.Include(a => a.School).FirstOrDefaultAsync(a => a.Id == accountId);
        Assert.That(updatedAccount, Is.Not.Null);
        Assert.That(updatedAccount.School, Is.Not.Null);
        Assert.That(updatedAccount.School.Id, Is.EqualTo(schoolResponse.Id));
    }

    [Test]
    public async Task ShouldNotThrowValidationException_WhenSchoolIdIsNotRequired()
    {
        var accountId = await CreateAccount(UserRole.Admin);

        var command = new UpdateAccountCommand
        {
            Id = accountId,
            Name = "Updated Admin Account",
            RegistrationNumber = "654321",
            AverageScore = 200.75m,
            EventAverageScore = 150.50m,
            Stars = 5,
            ClassIds = new List<Guid> { Guid.NewGuid() } // Add test class IDs
        };

        Assert.DoesNotThrowAsync(async () => await SendAsync(command));
    }
}