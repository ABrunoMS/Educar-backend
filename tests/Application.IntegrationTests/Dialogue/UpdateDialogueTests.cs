using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Dialogue.CreateDialogue;
using Educar.Backend.Application.Commands.Dialogue.UpdateDialogue;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Dialogue;

[TestFixture]
public class UpdateDialogueCommandTests : TestBase
{
    private Domain.Entities.Npc _npc;
    private Domain.Entities.Dialogue _dialogue;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _npc = new Domain.Entities.Npc("Test NPC", "Test lore", NpcType.Enemy, 0.5m, 100);
        Context.Npcs.Add(_npc);
        Context.SaveChanges();

        _dialogue = new Domain.Entities.Dialogue("Initial Dialogue", 1)
        {
            Npc = _npc
        };
        Context.Dialogues.Add(_dialogue);
        Context.SaveChanges();
    }

    [Test]
    public async Task GivenValidRequest_ShouldUpdateDialogue()
    {
        // Arrange
        const string newText = "Updated Dialogue";
        const int newOrder = 2;

        var createDialogueCommand = new CreateDialogueCommand("Another Dialogue", 2, _npc.Id);
        var response = await SendAsync(createDialogueCommand);
        
        var command = new UpdateDialogueCommand
        {
            Id = response.Id,
            Text = newText,
            Order = newOrder,
            NpcId = _npc.Id
        };

        // Act
        await SendAsync(command);

        // Assert
        var updatedDialogue = await Context.Dialogues
            .Include(d => d.Npc)
            .FirstOrDefaultAsync(d => d.Id == response.Id);

        Assert.That(updatedDialogue, Is.Not.Null);
        Assert.That(updatedDialogue.Text, Is.EqualTo(newText));
        Assert.That(updatedDialogue.Order, Is.EqualTo(newOrder));
        Assert.That(updatedDialogue.Npc.Id, Is.EqualTo(_npc.Id));
    }

    [Test]
    public void ShouldThrowValidationException_WhenTextIsEmpty()
    {
        var command = new UpdateDialogueCommand
        {
            Id = _dialogue.Id,
            Text = string.Empty,
            Order = 1,
            NpcId = _npc.Id
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenOrderIsLessThanOne()
    {
        var command = new UpdateDialogueCommand
        {
            Id = _dialogue.Id,
            Text = "Test Dialogue",
            Order = 0,
            NpcId = _npc.Id
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenOrderIsNotUniqueForNpc()
    {
        // Arrange: Create a dialogue with order 2
        var existingCommand = new UpdateDialogueCommand
        {
            Id = _dialogue.Id,
            Text = "Existing Dialogue",
            Order = 2,
            NpcId = _npc.Id
        };
        await SendAsync(existingCommand);

        // Act & Assert: Try to update another dialogue with the same order
        var command = new UpdateDialogueCommand
        {
            Id = Guid.NewGuid(),
            Text = "Test Dialogue",
            Order = 2,
            NpcId = _npc.Id
        };
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNpcIdIsEmpty()
    {
        var command = new UpdateDialogueCommand
        {
            Id = _dialogue.Id,
            Text = "Test Dialogue",
            Order = 1,
            NpcId = Guid.Empty
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowNotFoundException_WhenNpcIdIsInvalid()
    {
        var invalidNpcId = Guid.NewGuid();
        var command = new UpdateDialogueCommand
        {
            Id = _dialogue.Id,
            Text = "Test Dialogue",
            Order = 11,
            NpcId = invalidNpcId
        };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowNotFoundException_WhenIdIsInvalid()
    {
        var command = new UpdateDialogueCommand
        {
            Id = Guid.NewGuid(), // Invalid Id
            Text = "Test Dialogue",
            Order = 11,
            NpcId = _npc.Id
        };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }
}