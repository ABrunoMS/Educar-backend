using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Grade.CreateGradeCommand;
using Educar.Backend.Application.Commands.Grade.UpdateGrade;
using Educar.Backend.Application.Common.Exceptions;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Grade;

[TestFixture]
public class UpdateGradeTests : TestBase
{
    private const string ValidUpdatedName = "Updated Grade";
    private const string ValidUpdatedDescription = "Updated Description";
    private const string InitialGradeName = "Initial Grade";
    private const string InitialGradeDescription = "Initial Description";

    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldUpdateGrade()
    {
        // Arrange
        var createCommand = new CreateGradeCommand(InitialGradeName, InitialGradeDescription);
        var createResponse = await SendAsync(createCommand);

        var updateCommand = new UpdateGradeCommand
        {
            Id = createResponse.Id,
            Name = ValidUpdatedName,
            Description = ValidUpdatedDescription
        };

        // Act
        await SendAsync(updateCommand);

        // Assert
        var updatedGrade = await Context.Grades.FindAsync(updateCommand.Id);
        Assert.That(updatedGrade, Is.Not.Null);
        Assert.That(updatedGrade.Name, Is.EqualTo(ValidUpdatedName));
        Assert.That(updatedGrade.Description, Is.EqualTo(ValidUpdatedDescription));
    }

    [Test]
    public void ShouldThrowValidationException_WhenIdIsEmpty()
    {
        var command = new UpdateGradeCommand
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
        var command = new UpdateGradeCommand
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
        var command = new UpdateGradeCommand
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
        var command = new UpdateGradeCommand
        {
            Id = Guid.NewGuid(),
            Name = ValidUpdatedName,
            Description = ValidUpdatedDescription
        };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }
}