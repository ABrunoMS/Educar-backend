using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Account.CreateAccount;
using Educar.Backend.Application.Commands.Class.CreateClass;
using Educar.Backend.Application.Commands.Class.UpdateClass;
using Educar.Backend.Application.Commands.School.CreateSchool;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Class;

[TestFixture]
public class UpdateClassTests : TestBase
{
    private Domain.Entities.Client _client;
    private Domain.Entities.School _school;
    private Domain.Entities.Class _class;

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

        _class = new Domain.Entities.Class("Existing Class", "Class Description", ClassPurpose.Default)
        {
            School = _school
        };
        Context.Classes.Add(_class);
        Context.SaveChanges();
    }

    private async Task<Guid> CreateClass(Guid schoolId, string name = "New Class",
        string description = "Class Description")
    {
        var command = new CreateClassCommand(name, description, ClassPurpose.Default, schoolId);
        var response = await SendAsync(command);
        return response.Id;
    }

    [Test]
    public async Task GivenValidRequest_ShouldUpdateClass()
    {
        // Arrange
        const string newName = "Updated Class";
        const string newDescription = "Updated Description";
        const ClassPurpose newPurpose = ClassPurpose.Reinforcement;

        var schoolCommand = new CreateSchoolCommand("New School", _client.Id);
        var schoolResponse = await SendAsync(schoolCommand);

        var classId = await CreateClass(schoolResponse.Id);
        var account1 = await CreateAccount(UserRole.Student, schoolResponse.Id, new List<Guid> { classId });
        var account2 = await CreateAccount(UserRole.Student, schoolResponse.Id, new List<Guid> { classId });

        var command = new UpdateClassCommand
        {
            Id = classId,
            Name = newName,
            Description = newDescription,
            Purpose = newPurpose,
            AccountIds = [account1, account2]
        };

        // Act
        await SendAsync(command);

        // Assert
        var updatedClass = await Context.Classes.Include(c => c.AccountClasses)
            .FirstOrDefaultAsync(c => c.Id == classId);
        Assert.That(updatedClass, Is.Not.Null);
        Assert.That(updatedClass.Name, Is.EqualTo(newName));
        Assert.That(updatedClass.Description, Is.EqualTo(newDescription));
        Assert.That(updatedClass.Purpose, Is.EqualTo(newPurpose));
        Assert.That(updatedClass.AccountClasses.Select(ac => ac.AccountId), Is.EquivalentTo(command.AccountIds));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new UpdateClassCommand
        {
            Id = _class.Id,
            Name = string.Empty,
            Description = "Description",
            Purpose = ClassPurpose.Default
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenDescriptionIsEmpty()
    {
        var command = new UpdateClassCommand
        {
            Id = _class.Id,
            Name = "Class Name",
            Description = string.Empty,
            Purpose = ClassPurpose.Default
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenPurposeIsNone()
    {
        var command = new UpdateClassCommand
        {
            Id = _class.Id,
            Name = "Class Name",
            Description = "Description",
            Purpose = ClassPurpose.None
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenIdIsInvalid()
    {
        var command = new UpdateClassCommand
        {
            Id = Guid.NewGuid(), // Invalid Id
            Name = "Class Name",
            Description = "Description",
            Purpose = ClassPurpose.Default
        };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task GivenValidRequest_ShouldSoftDeleteAndReactivateAccountClasses()
    {
        var schoolCommand = new CreateSchoolCommand("New School", _client.Id);
        var schoolResponse = await SendAsync(schoolCommand);

        const string className = "class01";
        const string classDescription = "class01 description";

        var classId = await CreateClass(schoolResponse.Id, className, classDescription);
        var account1 = await CreateAccount(UserRole.Student, schoolResponse.Id, new List<Guid> { classId });
        var account2 = await CreateAccount(UserRole.Student, schoolResponse.Id, new List<Guid> { classId });
        var account3 = await CreateAccount(UserRole.Student, schoolResponse.Id, new List<Guid> { classId });


        var initialCommand = new UpdateClassCommand
        {
            Name = className,
            Description = classDescription,
            Id = classId,
            AccountIds = [account1, account2]
        };
        await SendAsync(initialCommand);

        var updateCommand = new UpdateClassCommand
        {
            Name = className,
            Description = classDescription,
            Id = classId,
            AccountIds = [account2, account3]
        };

        // Act
        await SendAsync(updateCommand);

        // Assert
        var updatedClass =
            await Context.Classes.Include(c => c.AccountClasses).FirstOrDefaultAsync(c => c.Id == classId);
        Assert.That(updatedClass, Is.Not.Null);
        Assert.That(updatedClass.AccountClasses, Has.Count.EqualTo(2));
        
        var accountClasses = Context.AccountClasses.IgnoreQueryFilters().Where(ac => ac.ClassId == classId).ToList();
        Assert.Multiple(() =>
        {
            //assert account1 is soft deleted
            Assert.That(accountClasses.Any(ac => ac.AccountId == account1 && ac.IsDeleted), Is.True);
            
            Assert.That(accountClasses.Any(ac => ac.AccountId == account2 && !ac.IsDeleted), Is.True);
            Assert.That(accountClasses.Any(ac => ac.AccountId == account3 && !ac.IsDeleted), Is.True);
        });
    }

    private async Task<Guid> CreateAccount(UserRole role = UserRole.Admin, Guid? schoolId = null,
        List<Guid>? classIds = null)
    {
        var email = $"{Guid.NewGuid()}@example.com";
        var command =
            new CreateAccountCommand("Existing Account", email, "123456", _client.Id, role)
            {
                AverageScore = 100.50m,
                EventAverageScore = 95.75m,
                Stars = 4
            };
        if (schoolId != null)
        {
            command.SchoolId = schoolId;
        }

        if (classIds != null)
        {
            command.ClassIds = classIds;
        }

        var response = await SendAsync(command);

        return response.Id;
    }
}