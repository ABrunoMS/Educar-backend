using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Contract.CreateContract;
using Educar.Backend.Application.Queries.Contract;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Contract;

[TestFixture]
public class GetContractTests : TestBase
{
    private Domain.Entities.Client _client;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        // Create and add a client to the context
        _client = new Domain.Entities.Client("test client");
        Context.Clients.Add(_client);
        Context.SaveChanges();
    }

    [Test]
    public async Task GivenValidRequest_ShouldReturnContract()
    {
        // Arrange
        var command = new CreateContractCommand(_client.Id)
        {
            ContractDurationInYears = 1,
            ContractSigningDate = DateTimeOffset.Now,
            ImplementationDate = DateTimeOffset.Now.AddMonths(1),
            TotalAccounts = 10,
            Status = ContractStatus.Signed
        };
        
        var response = await SendAsync(command);

        var query = new GetContractQuery { Id = response.Id };

        // Act
        var contractResponse = await SendAsync(query);

        // Assert
        Assert.That(contractResponse, Is.Not.Null);
        Assert.That(contractResponse.Id, Is.EqualTo(response.Id));
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetContractQuery { Id = Guid.NewGuid() };

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenDeletedResourceRequest_ShouldThrowNotFoundException()
    {
        // Arrange
        var contract = new Domain.Entities.Contract(1, DateTimeOffset.Now, DateTimeOffset.Now.AddMonths(1), 10, ContractStatus.Signed)
        {
            Client = _client,
            IsDeleted = true
        };
        Context.Contracts.Add(contract);
        await Context.SaveChangesAsync();

        var query = new GetContractQuery { Id = contract.Id };

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }
}
