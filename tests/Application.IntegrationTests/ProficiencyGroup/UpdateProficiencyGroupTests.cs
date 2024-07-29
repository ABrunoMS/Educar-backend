using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Proficiency.CreateProficiency;
using Educar.Backend.Application.Commands.ProficiencyGroup.CreateProficiencyGroup;
using Educar.Backend.Application.Commands.ProficiencyGroup.UpdateProficiencyGroup;
using Educar.Backend.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.ProficiencyGroup;

[TestFixture]
public class UpdateProficiencyGroupTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldUpdateProficiencyGroup()
    {
        // Arrange
        var createGroupCommand = new CreateProficiencyGroupCommand("Original Name", "Original Description");
        var createdGroupResponse = await SendAsync(createGroupCommand);
        var groupId = createdGroupResponse.Id;

        var updateProficiencyCommand =
            new CreateProficiencyCommand("Updated Proficiency", "Updated Description", "Updated Purpose");
        var updatedProficiencyResponse = await SendAsync(updateProficiencyCommand);

        var updateCommand = new UpdateProficiencyGroupCommand
        {
            Id = groupId,
            Name = "Updated Name",
            Description = "Updated Description",
            ProficiencyIds = new List<Guid> { updatedProficiencyResponse.Id }
        };

        // Act
        await SendAsync(updateCommand);

        // Assert
        if (Context.ProficiencyGroups != null)
        {
            var updatedGroup = await Context.ProficiencyGroups
                .Include(g => g.ProficiencyGroupProficiencies)
                .ThenInclude(pgp => pgp.Proficiency)
                .FirstOrDefaultAsync(g => g.Id == groupId);
            Assert.That(updatedGroup, Is.Not.Null);
            Assert.That(updatedGroup.Name, Is.EqualTo("Updated Name"));
            Assert.That(updatedGroup.Description, Is.EqualTo("Updated Description"));
            Assert.That(updatedGroup.ProficiencyGroupProficiencies, Has.Count.EqualTo(1));
            Assert.That(updatedGroup.ProficiencyGroupProficiencies.First().Proficiency.Name,
                Is.EqualTo("Updated Proficiency"));
        }
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        var updateCommand = new UpdateProficiencyGroupCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Name",
            Description = "Updated Description",
            ProficiencyIds = new List<Guid>()
        };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var updateCommand = new UpdateProficiencyGroupCommand
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            Description = "Updated Description",
            ProficiencyIds = new List<Guid>()
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var longName = new string('a', 101);
        var updateCommand = new UpdateProficiencyGroupCommand
        {
            Id = Guid.NewGuid(),
            Name = longName,
            Description = "Updated Description",
            ProficiencyIds = new List<Guid>()
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void ShouldThrowValidationException_WhenDescriptionIsEmpty()
    {
        var updateCommand = new UpdateProficiencyGroupCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Name",
            Description = string.Empty,
            ProficiencyIds = new List<Guid>()
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenNameIsNotUnique()
    {
        // Arrange
        var command1 = new CreateProficiencyGroupCommand("Unique Group", "Group Description");
        await SendAsync(command1);

        // Act
        var command2 = new UpdateProficiencyGroupCommand
        {
            Id = Guid.NewGuid(),
            Name = "Unique Group",
            Description = "Updated Description",
            ProficiencyIds = new List<Guid>()
        };

        // Assert
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command2));
    }
}