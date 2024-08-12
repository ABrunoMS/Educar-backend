using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Dialogue.CreateDialogue;
using Educar.Backend.Application.Queries.Dialogue;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Dialogue;

[TestFixture]
public class GetDialogueQueryTests : TestBase
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
    public async Task GivenValidRequest_ShouldReturnDialogue()
    {
        var createDialogueCommand = new CreateDialogueCommand("Test Dialogue", 1, _npc.Id);
        var createDialogueResponse = await SendAsync(createDialogueCommand);
        
        // Arrange
        var query = new GetDialogueQuery { Id = createDialogueResponse.Id };

        // Act
        var queryResponse = await SendAsync(query);

        // Assert
        Assert.That(queryResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(queryResponse.Id, Is.EqualTo(createDialogueResponse.Id));
            Assert.That(queryResponse.Text, Is.EqualTo("Test Dialogue"));
            Assert.That(queryResponse.Order, Is.EqualTo(1));
        });
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetDialogueQuery { Id = Guid.NewGuid() };

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenValidPaginationRequest_ShouldReturnPaginatedDialoguesByNpc()
    {
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var dialogue = new Domain.Entities.Dialogue($"Dialogue {i}", i)
            {
                Npc = _npc
            };
            Context.Dialogues.Add(dialogue);
        }

        await Context.SaveChangesAsync();

        var query = new GetDialoguesByNpcPaginatedQuery(_npc.Id) { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(10));
            Assert.That(result.PageNumber, Is.EqualTo(1));
            Assert.That(result.TotalCount, Is.EqualTo(20));
            Assert.That(result.TotalPages, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GivenSpecificPageRequest_ShouldReturnCorrectPageByNpc()
    {
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var dialogue = new Domain.Entities.Dialogue($"Dialogue {i}", i)
            {
                Npc = _npc
            };
            Context.Dialogues.Add(dialogue);
        }

        await Context.SaveChangesAsync();

        var query = new GetDialoguesByNpcPaginatedQuery(_npc.Id) { PageNumber = 2, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(10));
            Assert.That(result.PageNumber, Is.EqualTo(2));
            Assert.That(result.TotalCount, Is.EqualTo(20));
            Assert.That(result.TotalPages, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GivenOutOfRangePageRequest_ShouldReturnEmptyPage()
    {
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            var dialogue = new Domain.Entities.Dialogue($"Dialogue {i}", i)
            {
                Npc = _npc
            };
            Context.Dialogues.Add(dialogue);
        }

        await Context.SaveChangesAsync();

        var query = new GetDialoguesByNpcPaginatedQuery(_npc.Id) { PageNumber = 3, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Is.Empty);
            Assert.That(result.PageNumber, Is.EqualTo(3));
            Assert.That(result.TotalCount, Is.EqualTo(20));
            Assert.That(result.TotalPages, Is.EqualTo(2));
        });
    }
}