using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Proficiency.CreateProficiency;
using Educar.Backend.Application.Commands.Proficiency.UpdateProficiency;
using Educar.Backend.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Proficiency;

[TestFixture]
public class UpdateProficiencyTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldUpdateProficiency()
    {
        // Arrange
        var createCommand = new CreateProficiencyCommand("Original Name", "Original Description", "Original Purpose");
        var createdResponse = await SendAsync(createCommand);
        var proficiencyId = createdResponse.Id;

        var updateCommand = new UpdateProficiencyCommand
        {
            Id = proficiencyId,
            Name = "Updated Name",
            Description = "Updated Description",
            Purpose = "Updated Purpose"
        };

        // Act
        await SendAsync(updateCommand);

        // Assert
        if (Context.Proficiencies != null)
        {
            var updatedProficiency = await Context.Proficiencies.FirstOrDefaultAsync(p => p.Id == proficiencyId);
            Assert.That(updatedProficiency, Is.Not.Null);
            Assert.That(updatedProficiency.Name, Is.EqualTo("Updated Name"));
            Assert.That(updatedProficiency.Description, Is.EqualTo("Updated Description"));
            Assert.That(updatedProficiency.Purpose, Is.EqualTo("Updated Purpose"));
        }
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        var updateCommand = new UpdateProficiencyCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Name",
            Description = "Updated Description",
            Purpose = "Updated Purpose"
        };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var updateCommand = new UpdateProficiencyCommand
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            Description = "Updated Description",
            Purpose = "Updated Purpose"
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var longName = new string('a', 101);
        var updateCommand = new UpdateProficiencyCommand
        {
            Id = Guid.NewGuid(),
            Name = longName,
            Description = "Updated Description",
            Purpose = "Updated Purpose"
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenDescriptionIsEmpty()
    {
        var updateCommand = new UpdateProficiencyCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Name",
            Description = string.Empty,
            Purpose = "Updated Purpose"
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenPurposeIsEmpty()
    {
        var updateCommand = new UpdateProficiencyCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Name",
            Description = "Updated Description",
            Purpose = string.Empty
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenPurposeExceedsMaxLength()
    {
        var longPurpose = new string('a', 256);
        var updateCommand = new UpdateProficiencyCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Name",
            Description = "Updated Description",
            Purpose = longPurpose
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenNameIsNotUnique()
    {
        // Arrange
        var command1 = new CreateProficiencyCommand("Unique Proficiency", "Description", "Purpose");
        await SendAsync(command1);

        var createCommand = new CreateProficiencyCommand("Another Proficiency", "Description", "Purpose");
        var createdResponse = await SendAsync(createCommand);
        var proficiencyId = createdResponse.Id;

        // Act
        var updateCommand = new UpdateProficiencyCommand
        {
            Id = proficiencyId,
            Name = "Unique Proficiency",
            Description = "Updated Description",
            Purpose = "Updated Purpose"
        };

        // Assert
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }
}