using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Contract.CreateContract;
using Educar.Backend.Application.Commands.Game.CreateGame;
using Educar.Backend.Application.Commands.Game.DeleteGame;
using Educar.Backend.Application.Commands.Npc.CreateNpc;
using Educar.Backend.Application.Commands.ProficiencyGroup.CreateProficiencyGroup;
using Educar.Backend.Application.Commands.Subject.CreateSubject;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Game;

[TestFixture]
public class DeleteGameTests : TestBase
{
    // ... outros métodos de teste que não precisam de correção ...

    [Test]
    public async Task GivenGameWithContracts_ShouldThrowException()
    {
        // Arrange
        var createSubjectCommand = new CreateSubjectCommand("Test Subject", "Description");
        var createdSubjectResponse = await SendAsync(createSubjectCommand);

        var createProficiencyGroupCommand =
            new CreateProficiencyGroupCommand("Test ProficiencyGroup", "ProficiencyGroup Description");
        var createdProficiencyGroupResponse = await SendAsync(createProficiencyGroupCommand);

        var createGameCommand = new CreateGameCommand("Test Game", "Description", "Lore", "Purpose")
        {
            SubjectIds = new List<Guid> { createdSubjectResponse.Id },
            ProficiencyGroupIds = new List<Guid> { createdProficiencyGroupResponse.Id }
        };
        var createdGameResponse = await SendAsync(createGameCommand);

        // CORRIGIDO: Usando o helper para criar o cliente
        var clientId = await CreateClientAsAdminAsync("client");

        var createContractCommand = new CreateContractCommand(clientId, createdGameResponse.Id)
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