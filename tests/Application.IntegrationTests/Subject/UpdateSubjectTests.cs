using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Subject.CreateSubject;
using Educar.Backend.Application.Commands.Subject.UpdateSubject;
using Educar.Backend.Application.Common.Exceptions;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Subject;

[TestFixture]
public class UpdateSubjectTests : TestBase
{
    private const string ValidUpdatedName = "Updated Subject";
    private const string ValidUpdatedDescription = "Updated Description";
    private const string InitialSubjectName = "Initial Subject";
    private const string InitialSubjectDescription = "Initial Description";

    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldUpdateSubject()
    {
        // Arrange
        var createCommand = new CreateSubjectCommand(InitialSubjectName, InitialSubjectDescription);
        var createResponse = await SendAsync(createCommand);

        var updateCommand = new UpdateSubjectCommand
        {
            Id = createResponse.Id,
            Name = ValidUpdatedName,
            Description = ValidUpdatedDescription
        };

        // Act
        await SendAsync(updateCommand);

        // Assert
        var updatedSubject = await Context.Subjects.FindAsync(updateCommand.Id);
        Assert.That(updatedSubject, Is.Not.Null);
        Assert.That(updatedSubject.Name, Is.EqualTo(ValidUpdatedName));
        Assert.That(updatedSubject.Description, Is.EqualTo(ValidUpdatedDescription));
    }

    [Test]
    public void ShouldThrowValidationException_WhenIdIsEmpty()
    {
        var command = new UpdateSubjectCommand
        {
            Id = Guid.Empty,
            Name = ValidUpdatedName,
            Description = ValidUpdatedDescription
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new UpdateSubjectCommand
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            Description = ValidUpdatedDescription
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenDescriptionIsEmpty()
    {
        var command = new UpdateSubjectCommand
        {
            Id = Guid.NewGuid(),
            Name = ValidUpdatedName,
            Description = string.Empty
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        var command = new UpdateSubjectCommand
        {
            Id = Guid.NewGuid(),
            Name = ValidUpdatedName,
            Description = ValidUpdatedDescription
        };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }
}