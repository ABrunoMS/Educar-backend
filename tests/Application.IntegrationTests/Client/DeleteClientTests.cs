using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Client;
using Educar.Backend.Application.Commands.Client.CreateClient;
using Educar.Backend.Application.Commands.Client.DeleteClient;
using Educar.Backend.Application.Commands.Contract.CreateContract;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Client;

[TestFixture]
public class DeleteClientTests : TestBase
{
    private const string ClientName = "Test Client";
    private const string ClientDescription = "Test Description";

    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldSoftDeleteClient()
    {
        // Arrange
        var createCommand = new CreateClientCommand(ClientName)
        {
            Description = ClientDescription
        };
        var createResponse = await SendAsync(createCommand);

        var deleteCommand = new DeleteClientCommand(createResponse.Id);

        // Act
        await SendAsync(deleteCommand);

        // Assert
        var deletedClient =
            await Context.Clients.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == deleteCommand.Id);
        Assert.That(deletedClient, Is.Not.Null);
        Assert.That(deletedClient.IsDeleted, Is.True);
        Assert.That(deletedClient.DeletedAt, Is.Not.Null);
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        // Arrange
        var deleteCommand = new DeleteClientCommand(Guid.NewGuid());

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(deleteCommand));
    }

    [Test]
    public async Task GivenClientWithContract_ShouldSoftDeleteClientAndFireEvent()
    {
        // Arrange
        var createClientCommand = new CreateClientCommand(ClientName)
        {
            Description = ClientDescription
        };
        var createClientResponse = await SendAsync(createClientCommand);

        var createContractCommand = new CreateContractCommand(createClientResponse.Id)
        {
            ContractDurationInYears = 1,
            ContractSigningDate = DateTimeOffset.Now,
            ImplementationDate = DateTimeOffset.Now.AddMonths(1),
            TotalAccounts = 10,
            Status = ContractStatus.Signed
        };

        var createdContract = await SendAsync(createContractCommand);

        var deleteCommand = new DeleteClientCommand(createClientResponse.Id);

        // Act
        await SendAsync(deleteCommand);

        // Assert
        var deletedClient =
            await Context.Clients.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == deleteCommand.Id);
        Assert.That(deletedClient, Is.Not.Null);
        Assert.That(deletedClient.IsDeleted, Is.True);
        Assert.That(deletedClient.DeletedAt, Is.Not.Null);

        // Verify the domain event was published
        var deletedContract = await Context.Contracts.IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == createdContract.Id);
        Assert.That(deletedContract, Is.Not.Null);
        Assert.That(deletedContract.IsDeleted, Is.True);
    }
}