using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Account.CreateAccount;
using Educar.Backend.Application.Commands.Class.CreateClass;
using Educar.Backend.Application.Commands.Class.DeleteClass;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Class;

[TestFixture]
public class DeleteClassTests : TestBase
{
    private const string ClassName = "Test Class";
    private const string SchoolName = "Test School";
    private Domain.Entities.School _school;

    [SetUp]
    public void SetUp()
    {
        ResetState();
        _school = new Domain.Entities.School(SchoolName);
        Context.Schools.Add(_school);
        Context.SaveChanges();
    }

    [Test]
    public async Task GivenValidRequest_ShouldSoftDeleteClass()
    {
        // Este teste não precisou de correção
        var createCommand = new CreateClassCommand(ClassName, "description", ClassPurpose.Default, _school.Id);
        var createResponse = await SendAsync(createCommand);
        var deleteCommand = new DeleteClassCommand(createResponse.Id);
        
        await SendAsync(deleteCommand);
        
        var deletedClass = await Context.Classes.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == deleteCommand.Id);
        Assert.That(deletedClass, Is.Not.Null);
        Assert.That(deletedClass.IsDeleted, Is.True);
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        // Este teste não precisou de correção
        var deleteCommand = new DeleteClassCommand(Guid.NewGuid());
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(deleteCommand));
    }

    [Test]
    public async Task GivenClassWithAccountClasses_ShouldSoftDeleteClassAndAccountClasses()
    {
        // Arrange
        var createClassCommand = new CreateClassCommand(ClassName, "description", ClassPurpose.Default, _school.Id);
        var createClassResponse = await SendAsync(createClassCommand);

        // CORRIGIDO: Usando o helper para criar o cliente
        var clientId = await CreateClientAsAdminAsync("Test Client");

        var accountCommand =
            new CreateAccountCommand("Test Account", "email@email.com", "001", clientId, UserRole.Student)
            {
                SchoolId = _school.Id,
                ClassIds = new List<Guid> { createClassResponse.Id }
            };
        var accountResponse = await SendAsync(accountCommand);

        var deleteCommand = new DeleteClassCommand(createClassResponse.Id);

        // Act
        await SendAsync(deleteCommand);

        // Assert
        var deletedClass = await Context.Classes.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == deleteCommand.Id);
        Assert.That(deletedClass, Is.Not.Null);
        Assert.That(deletedClass.IsDeleted, Is.True);

        var deletedAccountClass = await Context.AccountClasses.IgnoreQueryFilters()
            .FirstOrDefaultAsync(ac => ac.ClassId == createClassResponse.Id && ac.AccountId == accountResponse.Id);
        Assert.That(deletedAccountClass, Is.Not.Null);
        Assert.That(deletedAccountClass.IsDeleted, Is.True);
    }
}