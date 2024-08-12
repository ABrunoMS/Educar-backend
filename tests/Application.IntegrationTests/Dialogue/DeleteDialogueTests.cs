using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Dialogue.CreateDialogue;
using Educar.Backend.Application.Commands.Dialogue.DeleteDialogue;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Dialogue;

[TestFixture]
public class DeleteDialogueCommandTests : TestBase
{
    private Domain.Entities.Npc _npc;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _npc = new Domain.Entities.Npc("Test NPC", "Test lore", NpcType.Enemy, 0.5m, 100);
        Context.Npcs.Add(_npc);
        Context.SaveChanges();
    }

    [Test]
    public async Task GivenValidRequest_ShouldSoftDeleteDialogue()
    {
        var createCommand = new CreateDialogueCommand("Test dialogue", 1, _npc.Id);
        var createResponse = await SendAsync(createCommand);

        // Arrange
        var deleteCommand = new DeleteDialogueCommand(createResponse.Id);

        // Act
        await SendAsync(deleteCommand);

        // Assert
        var deletedDialogue =
            await Context.Dialogues.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.Id == createResponse.Id);
        Assert.That(deletedDialogue, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(deletedDialogue.IsDeleted, Is.True);
            Assert.That(deletedDialogue.DeletedAt, Is.Not.Null);
        });
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        var deleteCommand = new DeleteDialogueCommand(invalidId);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(deleteCommand));
    }
}