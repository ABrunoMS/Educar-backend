using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Proficiency.CreateProficiency;
using Educar.Backend.Application.Commands.Proficiency.DeleteProficiency;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Proficiency;

[TestFixture]
public class DeleteProficiencyTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidId_ShouldDeleteProficiency()
    {
        // Arrange
        var createCommand = new CreateProficiencyCommand("Test Proficiency", "Description", "Purpose");
        var createdResponse = await SendAsync(createCommand);
        var proficiencyId = createdResponse.Id;

        var deleteCommand = new DeleteProficiencyCommand(proficiencyId);

        // Act
        await SendAsync(deleteCommand);

        // Assert
        var deletedProficiency =
            await Context.Proficiencies.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == proficiencyId);
        Assert.That(deletedProficiency, Is.Not.Null);
        Assert.That(deletedProficiency.IsDeleted, Is.True);

        // Ensure the associated proficiency group proficiencies are also soft-deleted
        var deletedProficiencyGroupProficiencies =
            await Context.ProficiencyGroupProficiencies.IgnoreQueryFilters()
                .Where(pg => pg.ProficiencyId == proficiencyId).ToListAsync();
        Assert.That(deletedProficiencyGroupProficiencies.All(pg => pg.IsDeleted), Is.True);
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        var deleteCommand = new DeleteProficiencyCommand(Guid.NewGuid());

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(deleteCommand));
    }
}