using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Client.CreateClient;
using Educar.Backend.Application.Commands.Contract.CreateContract;
using Educar.Backend.Application.Queries.Client;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;

namespace Educar.Backend.Application.IntegrationTests.Client;

using static Testing;

[TestFixture]
public class GetClientTests : TestBase
{
    private const string ClientName = "Test Client";
    private const string ClientDescription = "Test Description";
    private const int ContractDurationInYears = 1;
    private const int TotalAccounts = 10;
    private const ContractStatus ContractStatus = Domain.Enums.ContractStatus.Signed;

    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldReturnClient()
    {
        // Arrange
        var createCommand = new CreateClientCommand(ClientName)
        {
            Description = ClientDescription
        };
        var createResponse = await SendAsync(createCommand);

        var query = new GetClientQuery { Id = createResponse.Id };

        // Act
        var clientResponse = await SendAsync(query);

        // Assert
        Assert.That(clientResponse, Is.Not.Null);
        Assert.That(clientResponse.Id, Is.EqualTo(createResponse.Id));
        Assert.That(clientResponse.Name, Is.EqualTo(ClientName));
        Assert.That(clientResponse.Description, Is.EqualTo(ClientDescription));
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetClientQuery { Id = Guid.NewGuid() };

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenClientWithContract_ShouldReturnClientWithContract()
    {
        // Arrange
        var createClientCommand = new CreateClientCommand(ClientName)
        {
            Description = ClientDescription
        };
        var createClientResponse = await SendAsync(createClientCommand);

        var createContractCommand = new CreateContractCommand(createClientResponse.Id)
        {
            ContractDurationInYears = ContractDurationInYears,
            ContractSigningDate = DateTimeOffset.Now,
            ImplementationDate = DateTimeOffset.Now.AddMonths(1),
            TotalAccounts = TotalAccounts,
            Status = ContractStatus
        };
        var createContractResponse = await SendAsync(createContractCommand);

        var query = new GetClientQuery { Id = createClientResponse.Id };

        // Act
        var clientResponse = await SendAsync(query);

        // Assert
        Assert.That(clientResponse, Is.Not.Null);
        Assert.That(clientResponse.Id, Is.EqualTo(createClientResponse.Id));
        Assert.That(clientResponse.Name, Is.EqualTo(ClientName));
        Assert.That(clientResponse.Description, Is.EqualTo(ClientDescription));
        Assert.That(clientResponse.Contracts, Is.Not.Empty);
        Assert.That(clientResponse.Contracts.First().Id, Is.EqualTo(createContractResponse.Id));
    }
}