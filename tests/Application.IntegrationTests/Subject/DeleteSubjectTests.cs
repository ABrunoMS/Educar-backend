using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Game.CreateGame;
using Educar.Backend.Application.Commands.Subject.CreateSubject;
using Educar.Backend.Application.Commands.Subject.DeleteSubject;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Subject;

[TestFixture]
public class DeleteSubjectTests : TestBase
{
    private const string SubjectName = "Test Subject";
    private const string SubjectDescription = "Test Description";

    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldSoftDeleteSubject()
    {
        // Arrange
        var createCommand = new CreateSubjectCommand(SubjectName, SubjectDescription);
        var createResponse = await SendAsync(createCommand);

        var deleteCommand = new DeleteSubjectCommand(createResponse.Id);

        // Act
        await SendAsync(deleteCommand);

        // Assert
        var deletedSubject =
            await Context.Subjects.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == deleteCommand.Id);
        Assert.That(deletedSubject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(deletedSubject.IsDeleted, Is.True);
            Assert.That(deletedSubject.DeletedAt, Is.Not.Null);
        });
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        // Arrange
        var deleteCommand = new DeleteSubjectCommand(Guid.NewGuid());

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(deleteCommand));
    }

    [Test]
    public async Task GivenSubjectWithGameSubjects_ShouldSoftDeleteSubjectAndGameSubjects()
    {
        // Arrange
        var createSubjectCommand = new CreateSubjectCommand(SubjectName, SubjectDescription);
        var createSubjectResponse = await SendAsync(createSubjectCommand);

        var createGameCommand = new CreateGameCommand("Test Game", "Test Description", "Lore", "purpose")
        {
            SubjectIds = new List<Guid> { createSubjectResponse.Id }
        };
        var createGameResponse = await SendAsync(createGameCommand);

        var deleteCommand = new DeleteSubjectCommand(createSubjectResponse.Id);

        // Act
        await SendAsync(deleteCommand);

        // Assert
        var deletedSubject =
            await Context.Subjects.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == deleteCommand.Id);
        Assert.That(deletedSubject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(deletedSubject.IsDeleted, Is.True);
            Assert.That(deletedSubject.DeletedAt, Is.Not.Null);
        });

        // Verify the associated GameSubjects are soft-deleted
        var deletedGameSubject = await Context.GameSubjects.IgnoreQueryFilters()
            .FirstOrDefaultAsync(gs => gs.SubjectId == createSubjectResponse.Id && gs.GameId == createGameResponse.Id);
        Assert.That(deletedGameSubject, Is.Not.Null);
        Assert.That(deletedGameSubject.IsDeleted, Is.True);
    }
}