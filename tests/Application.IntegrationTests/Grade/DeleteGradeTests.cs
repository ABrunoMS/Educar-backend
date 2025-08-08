using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Grade.CreateGradeCommand;
using Educar.Backend.Application.Commands.Grade.DeleteGrade;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Grade;

[TestFixture]
public class DeleteGradeTests : TestBase
{
    private const string GradeName = "Test Grade";
    private const string GradeDescription = "Test Description";

    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldDeleteGrade()
    {
        // Arrange
        var createCommand = new CreateGradeCommand(GradeName, GradeDescription);
        var createResponse = await SendAsync(createCommand);

        var deleteCommand = new DeleteGradeCommand(createResponse.Id);

        // Act
        await SendAsync(deleteCommand);

        // Assert
        var deletedGrade = await Context.Grades.FirstOrDefaultAsync(g => g.Id == deleteCommand.Id);
        Assert.That(deletedGrade, Is.Null);

        var softDeletedGrade =
            await Context.Grades.IgnoreQueryFilters().FirstOrDefaultAsync(g => g.Id == deleteCommand.Id);
        Assert.That(softDeletedGrade, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(softDeletedGrade.IsDeleted, Is.True);
            Assert.That(softDeletedGrade.DeletedAt, Is.Not.Null);
        });
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        // Arrange
        var deleteCommand = new DeleteGradeCommand(Guid.NewGuid());

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(deleteCommand));
    }
}