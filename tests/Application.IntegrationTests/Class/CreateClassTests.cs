using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Class.CreateClass;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Class;

[TestFixture]
public class CreateClassCommandTests : TestBase
{
    private Domain.Entities.School _school;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _school = new Domain.Entities.School("test school");
        Context.Schools.Add(_school);

        Context.SaveChanges();
    }

    [Test]
    public async Task GivenValidRequest_ShouldCreateClass()
    {
        // Arrange
        var command = new CreateClassCommand("test class", "test description", ClassPurpose.Default, _school.Id);

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<IdResponseDto>());

        var createdClass = await Context.Classes
            .Include(c => c.School)
            .FirstOrDefaultAsync(c => c.Id == response.Id);

        Assert.That(createdClass, Is.Not.Null);
        Assert.That(createdClass.Id, Is.Not.Empty);
        Assert.That(createdClass.School.Id, Is.EqualTo(_school.Id));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new CreateClassCommand("", "test description", ClassPurpose.Default, _school.Id);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var command =
            new CreateClassCommand(new string('a', 101), "test description", ClassPurpose.Default, _school.Id);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenDescriptionIsEmpty()
    {
        var command = new CreateClassCommand("test class", "", ClassPurpose.Default, _school.Id);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenPurposeIsInvalid()
    {
        var command = new CreateClassCommand("test class", "test description", (ClassPurpose)999, _school.Id);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenPurposeIsNone()
    {
        var command = new CreateClassCommand("test class", "test description", ClassPurpose.None, _school.Id);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenSchoolIdIsEmpty()
    {
        var command = new CreateClassCommand("test class", "test description", ClassPurpose.Default, Guid.Empty);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowNotFoundException_WhenSchoolIdIsInvalid()
    {
        var invalidSchoolId = Guid.NewGuid();
        var command = new CreateClassCommand("test class", "test description", ClassPurpose.Default, invalidSchoolId);

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }
}