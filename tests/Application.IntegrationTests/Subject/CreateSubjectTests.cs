using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Subject.CreateSubject;
using Educar.Backend.Application.Common.Exceptions;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Subject;

[TestFixture]
public class CreateSubjectTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldCreateSubject()
    {
        // Arrange
        var command = new CreateSubjectCommand("New Subject", "Subject Description");

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<CreatedResponseDto>());

        if (Context.Subjects != null)
        {
            var createdSubject = await Context.Subjects.FindAsync(response.Id);
            Assert.That(createdSubject, Is.Not.Null);
            Assert.That(createdSubject.Name, Is.EqualTo("New Subject"));
            Assert.That(createdSubject.Description, Is.EqualTo("Subject Description"));
        }
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new CreateSubjectCommand(string.Empty, "Subject Description");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenDescriptionIsEmpty()
    {
        var command = new CreateSubjectCommand("New Subject", string.Empty);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var longName = new string('A', 151); // Name with 151 characters
        var command = new CreateSubjectCommand(longName, "Subject Description");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}