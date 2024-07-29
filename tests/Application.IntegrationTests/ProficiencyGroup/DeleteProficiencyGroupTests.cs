using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Proficiency.CreateProficiency;
using Educar.Backend.Application.Commands.ProficiencyGroup.CreateProficiencyGroup;
using Educar.Backend.Application.Commands.ProficiencyGroup.DeleteProficiencyGroup;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.ProficiencyGroup;

[TestFixture]
public class DeleteProficiencyGroupTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidId_ShouldDeleteProficiencyGroup()
    {
        // Arrange
        var createProficiencyCommand = new CreateProficiencyCommand("Test Proficiency", "Description", "Purpose");
        var createdProficiencyResponse = await SendAsync(createProficiencyCommand);

        var createGroupCommand = new CreateProficiencyGroupCommand("Test Proficiency Group", "Group Description")
        {
            ProficiencyIds = new List<Guid> { createdProficiencyResponse.Id }
        };
        var createdGroupResponse = await SendAsync(createGroupCommand);
        var groupId = createdGroupResponse.Id;

        var deleteCommand = new DeleteProficiencyGroupCommand(groupId);

        // Act
        await SendAsync(deleteCommand);

        // Assert
        var deletedGroup =
            await Context.ProficiencyGroups.IgnoreQueryFilters().FirstOrDefaultAsync(g => g.Id == groupId);
        Assert.That(deletedGroup, Is.Not.Null);
        Assert.That(deletedGroup.IsDeleted, Is.True);

        // Ensure the associated proficiencies are also removed from the group
        var deletedProficiencyGroupProficiencies =
            await Context.ProficiencyGroupProficiencies.IgnoreQueryFilters()
                .Where(pg => pg.ProficiencyGroupId == groupId).ToListAsync();
        Assert.That(deletedProficiencyGroupProficiencies, Is.Not.Empty);
        Assert.That(deletedProficiencyGroupProficiencies.First().IsDeleted, Is.True);
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        var deleteCommand = new DeleteProficiencyGroupCommand(Guid.NewGuid());

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(deleteCommand));
    }
}