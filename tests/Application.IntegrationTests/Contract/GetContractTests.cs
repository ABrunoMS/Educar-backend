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
    private Domain.Entities.Game _game;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _client = new Domain.Entities.Client("test client");
        Context.Clients.Add(_client);

        _game = new Domain.Entities.Game("test game", "test description", "lore", "test");
        Context.Games.Add(_game);

        Context.SaveChanges();
    }

    [Test]
    public async Task GivenValidRequest_ShouldReturnContract()
    {
        // Arrange
        var command = new CreateContractCommand(_client.Id, _game.Id)
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
        var contract = new Domain.Entities.Contract(1, DateTimeOffset.Now, DateTimeOffset.Now.AddMonths(1), 10,
            ContractStatus.Signed)
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

    [Test]
    public async Task GivenValidPaginationRequest_ShouldReturnPaginatedContracts()
    {
        // Arrange
        for (int i = 1; i <= 20; i++)
        {
            var command = new CreateContractCommand(_client.Id, _game.Id)
            {
                ContractDurationInYears = 1,
                ContractSigningDate = DateTimeOffset.Now,
                ImplementationDate = DateTimeOffset.Now.AddMonths(1),
                TotalAccounts = 10,
                Status = ContractStatus.Signed
            };
            await SendAsync(command);
        }

        var query = new GetContractsPaginatedQuery { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items.Count, Is.EqualTo(10));
        Assert.That(result.PageNumber, Is.EqualTo(1));
        Assert.That(result.TotalCount, Is.EqualTo(20));
        Assert.That(result.TotalPages, Is.EqualTo(2));
    }

    [Test]
    public async Task GivenSpecificPageRequest_ShouldReturnCorrectPage()
    {
        // Arrange
        for (int i = 1; i <= 20; i++)
        {
            var command = new CreateContractCommand(_client.Id, _game.Id)
            {
                ContractDurationInYears = 1,
                ContractSigningDate = DateTimeOffset.Now,
                ImplementationDate = DateTimeOffset.Now.AddMonths(1),
                TotalAccounts = 10,
                Status = ContractStatus.Signed
            };
            await SendAsync(command);
        }

        var query = new GetContractsPaginatedQuery { PageNumber = 2, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items.Count, Is.EqualTo(10));
        Assert.That(result.PageNumber, Is.EqualTo(2));
        Assert.That(result.TotalCount, Is.EqualTo(20));
        Assert.That(result.TotalPages, Is.EqualTo(2));
        Assert.That(result.Items.First().ContractDurationInYears, Is.EqualTo(1));
    }

    [Test]
    public async Task GivenOutOfRangePageRequest_ShouldReturnEmptyPage()
    {
        // Arrange
        for (int i = 1; i <= 20; i++)
        {
            var command = new CreateContractCommand(_client.Id, _game.Id)
            {
                ContractDurationInYears = 1,
                ContractSigningDate = DateTimeOffset.Now,
                ImplementationDate = DateTimeOffset.Now.AddMonths(1),
                TotalAccounts = 10,
                Status = ContractStatus.Signed
            };
            await SendAsync(command);
        }

        var query = new GetContractsPaginatedQuery { PageNumber = 3, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items.Count, Is.EqualTo(0));
        Assert.That(result.PageNumber, Is.EqualTo(3));
        Assert.That(result.TotalCount, Is.EqualTo(20));
        Assert.That(result.TotalPages, Is.EqualTo(2));
    }
}