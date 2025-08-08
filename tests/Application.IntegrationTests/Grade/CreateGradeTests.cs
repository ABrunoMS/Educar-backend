using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Grade.CreateGradeCommand;
using Educar.Backend.Application.Common.Exceptions;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Grade;

[TestFixture]
public class CreateGradeTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldCreateGrade()
    {
        // Arrange
        var command = new CreateGradeCommand("New Grade", "Grade Description");

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<IdResponseDto>());

        if (Context.Grades != null)
        {
            var createdGrade = await Context.Grades.FindAsync(response.Id);
            Assert.That(createdGrade, Is.Not.Null);
            Assert.That(createdGrade.Name, Is.EqualTo("New Grade"));
            Assert.That(createdGrade.Description, Is.EqualTo("Grade Description"));
        }
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new CreateGradeCommand(string.Empty, "Grade Description");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenDescriptionIsEmpty()
    {
        var command = new CreateGradeCommand("New Grade", string.Empty);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var longName = new string('A', 151); // Name with 151 characters
        var command = new CreateGradeCommand(longName, "Grade Description");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}