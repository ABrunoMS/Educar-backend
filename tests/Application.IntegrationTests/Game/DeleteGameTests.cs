using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Client.CreateClient;
using Educar.Backend.Application.Commands.Contract.CreateContract;
using Educar.Backend.Application.Commands.Game.CreateGame;
using Educar.Backend.Application.Commands.Game.DeleteGame;
using Educar.Backend.Application.Commands.Subject.CreateSubject;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Game;

[TestFixture]
public class DeleteGameTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidId_ShouldDeleteGame()
    {
        // Arrange
        var createSubjectCommand = new CreateSubjectCommand("Test Subject", "Description");
        var createdSubjectResponse = await SendAsync(createSubjectCommand);

        var createCommand = new CreateGameCommand("Test Game", "Description", "Lore", "Purpose")
            { SubjectIds = new List<Guid> { createdSubjectResponse.Id } };
        var createdResponse = await SendAsync(createCommand);
        var gameId = createdResponse.Id;

        var deleteCommand = new DeleteGameCommand(gameId);

        // Act
        await SendAsync(deleteCommand);

        // Assert
        var deletedGame = await Context.Games.IgnoreQueryFilters().FirstOrDefaultAsync(g => g.Id == gameId);
        Assert.That(deletedGame, Is.Not.Null);
        Assert.That(deletedGame.IsDeleted, Is.True);

        // Ensure the associated subjects are also soft-deleted
        var deletedGameSubjects =
            await Context.GameSubjects.IgnoreQueryFilters().Where(gs => gs.GameId == gameId).ToListAsync();
        Assert.That(deletedGameSubjects.All(gs => gs.IsDeleted), Is.True);
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        var deleteCommand = new DeleteGameCommand(Guid.NewGuid());

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(deleteCommand));
    }

    [Test]
    public async Task GivenGameWithContracts_ShouldThrowException()
    {
        // Arrange
        var createSubjectCommand = new CreateSubjectCommand("Test Subject", "Description");
        var createdSubjectResponse = await SendAsync(createSubjectCommand);

        var createGameCommand = new CreateGameCommand("Test Game", "Description", "Lore", "Purpose")
            { SubjectIds = new List<Guid> { createdSubjectResponse.Id } };
        var createdGameResponse = await SendAsync(createGameCommand);

        var createClientCommand = new CreateClientCommand("client");
        var createdClientResponse = await SendAsync(createClientCommand);

        var createContractCommand = new CreateContractCommand(createdClientResponse.Id, createdGameResponse.Id)
        {
            ContractDurationInYears = 1,
            ContractSigningDate = DateTimeOffset.UtcNow,
            ImplementationDate = DateTimeOffset.UtcNow,
            TotalAccounts = 10,
            Status = ContractStatus.Signed
        };
        await SendAsync(createContractCommand);

        var deleteCommand = new DeleteGameCommand(createdGameResponse.Id);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await SendAsync(deleteCommand),
            "This game has contracts and cannot be deleted.");
    }
}