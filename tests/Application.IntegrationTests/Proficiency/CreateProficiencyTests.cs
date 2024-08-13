using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Proficiency.CreateProficiency;
using Educar.Backend.Application.Common.Exceptions;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Proficiency;

[TestFixture]
public class CreateProficiencyTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldCreateProficiency()
    {
        const string name = "New Proficiency";
        const string description = "Proficiency Description";
        const string purpose = "Proficiency Purpose";

        // Arrange
        var command = new CreateProficiencyCommand(name, description, purpose);

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<IdResponseDto>());

        if (Context.Proficiencies != null)
        {
            var createdProficiency = await Context.Proficiencies.FindAsync(response.Id);
            Assert.That(createdProficiency, Is.Not.Null);
            Assert.That(createdProficiency.Name, Is.EqualTo(name));
            Assert.That(createdProficiency.Description, Is.EqualTo(description));
            Assert.That(createdProficiency.Purpose, Is.EqualTo(purpose));
        }
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new CreateProficiencyCommand(string.Empty, "Proficiency Description", "Proficiency Purpose");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var longName = new string('a', 101);
        var command = new CreateProficiencyCommand(longName, "Proficiency Description", "Proficiency Purpose");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenNameIsNotUnique()
    {
        // Arrange
        var command1 =
            new CreateProficiencyCommand("Unique Proficiency", "Proficiency Description", "Proficiency Purpose");
        await SendAsync(command1);

        // Act
        var command2 = new CreateProficiencyCommand("Unique Proficiency", "Another Description", "Another Purpose");

        // Assert
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command2));
    }

    [Test]
    public void ShouldThrowValidationException_WhenDescriptionIsEmpty()
    {
        var command = new CreateProficiencyCommand("Proficiency Name", string.Empty, "Proficiency Purpose");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenPurposeIsEmpty()
    {
        var command = new CreateProficiencyCommand("Proficiency Name", "Proficiency Description", string.Empty);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenPurposeExceedsMaxLength()
    {
        var longPurpose = new string('a', 256);
        var command = new CreateProficiencyCommand("Proficiency Name", "Proficiency Description", longPurpose);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}