using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Proficiency.CreateProficiency;
using Educar.Backend.Application.Commands.ProficiencyGroup.CreateProficiencyGroup;
using Educar.Backend.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.ProficiencyGroup;

[TestFixture]
public class CreateProficiencyGroupTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldCreateProficiencyGroup()
    {
        const string name = "New Proficiency Group";
        const string description = "Proficiency Group Description";

        // Arrange
        var createProficiencyCommand =
            new CreateProficiencyCommand("Test Proficiency", "Proficiency Description", "Proficiency Purpose");
        var createdProficiencyResponse = await SendAsync(createProficiencyCommand);

        var command = new CreateProficiencyGroupCommand(name, description)
        {
            ProficiencyIds = new List<Guid> { createdProficiencyResponse.Id }
        };

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<IdResponseDto>());

        if (Context.ProficiencyGroups != null)
        {
            var createdGroup = await Context.ProficiencyGroups
                .Include(pg => pg.ProficiencyGroupProficiencies)
                .ThenInclude(pgp => pgp.Proficiency)
                .FirstOrDefaultAsync(pg => pg.Id == response.Id);
            Assert.That(createdGroup, Is.Not.Null);
            Assert.That(createdGroup.Name, Is.EqualTo(name));
            Assert.That(createdGroup.Description, Is.EqualTo(description));
            Assert.That(createdGroup.ProficiencyGroupProficiencies, Has.Count.EqualTo(1));
            Assert.That(createdGroup.ProficiencyGroupProficiencies.First().Proficiency.Name,
                Is.EqualTo("Test Proficiency"));
        }
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new CreateProficiencyGroupCommand(string.Empty, "Proficiency Group Description");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var longName = new string('a', 101);
        var command = new CreateProficiencyGroupCommand(longName, "Proficiency Group Description");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenNameIsNotUnique()
    {
        // Arrange
        var command1 = new CreateProficiencyGroupCommand("Unique Group", "Proficiency Group Description");
        await SendAsync(command1);

        // Act
        var command2 = new CreateProficiencyGroupCommand("Unique Group", "Another Description");

        // Assert
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command2));
    }

    [Test]
    public void ShouldThrowValidationException_WhenDescriptionIsEmpty()
    {
        var command = new CreateProficiencyGroupCommand("Proficiency Group Name", string.Empty);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}