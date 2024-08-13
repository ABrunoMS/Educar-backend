using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Dialogue.CreateDialogue;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Dialogue;

[TestFixture]
public class CreateDialogueCommandTests : TestBase
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
    public async Task GivenValidRequest_ShouldCreateDialogue()
    {
        // Arrange
        var command = new CreateDialogueCommand("Test dialogue", 1, _npc.Id);

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<IdResponseDto>());

        var createdDialogue = await Context.Dialogues
            .Include(d => d.Npc)
            .FirstOrDefaultAsync(d => d.Id == response.Id);

        Assert.That(createdDialogue, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createdDialogue.Id, Is.Not.Empty);
            Assert.That(createdDialogue.Npc.Id, Is.EqualTo(_npc.Id));
            Assert.That(createdDialogue.Text, Is.EqualTo("Test dialogue"));
            Assert.That(createdDialogue.Order, Is.EqualTo(1));
        });
    }

    [Test]
    public void ShouldThrowValidationException_WhenTextIsEmpty()
    {
        var command = new CreateDialogueCommand("", 1, _npc.Id);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenOrderIsLessThanOne()
    {
        var command = new CreateDialogueCommand("Test dialogue", 0, _npc.Id);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenOrderIsNotUniqueForNpc()
    {
        // Arrange: Create a dialogue with order 1
        var existingCommand = new CreateDialogueCommand("Existing dialogue", 1, _npc.Id);
        await SendAsync(existingCommand);

        // Act & Assert: Try to create another dialogue with the same order
        var command = new CreateDialogueCommand("Test dialogue", 1, _npc.Id);
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNpcIdIsEmpty()
    {
        var command = new CreateDialogueCommand("Test dialogue", 1, Guid.Empty);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowNotFoundException_WhenNpcIdIsInvalid()
    {
        var invalidNpcId = Guid.NewGuid();
        var command = new CreateDialogueCommand("Test dialogue", 1, invalidNpcId);

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }
}