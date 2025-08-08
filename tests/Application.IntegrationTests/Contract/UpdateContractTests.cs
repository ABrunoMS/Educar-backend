using Educar.Backend.Application.Commands.Contract.CreateContract;
using Educar.Backend.Application.Commands.Contract.UpdateContract;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Contract;

[TestFixture]
public class UpdateContractTests : TestBase
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
    public async Task GivenValidRequest_ShouldUpdateContract()
    {
        // Arrange
        var createCommand = new CreateContractCommand(_client.Id, _game.Id)
        {
            ContractDurationInYears = 1,
            ContractSigningDate = DateTimeOffset.Now,
            ImplementationDate = DateTimeOffset.Now.AddMonths(1),
            TotalAccounts = 10,
            Status = ContractStatus.Signed
        };
        var createResponse = await SendAsync(createCommand);

        var updateCommand = new UpdateContractCommand
        {
            Id = createResponse.Id,
            ContractDurationInYears = 2,
            ContractSigningDate = DateTimeOffset.Now,
            ImplementationDate = DateTimeOffset.Now.AddMonths(2),
            TotalAccounts = 15,
            RemainingAccounts = 5,
            DeliveryReport = "Updated report",
            Status = ContractStatus.Signed
        };

        // Act
        await SendAsync(updateCommand);

        // Assert
        var updatedContract = await Context.Contracts.FindAsync(updateCommand.Id);
        Assert.That(updatedContract, Is.Not.Null);
        Assert.That(updatedContract.ContractDurationInYears, Is.EqualTo(2));
        Assert.That(updatedContract.ImplementationDate, Is.EqualTo(updateCommand.ImplementationDate));
        Assert.That(updatedContract.TotalAccounts, Is.EqualTo(15));
        Assert.That(updatedContract.RemainingAccounts, Is.EqualTo(5));
        Assert.That(updatedContract.DeliveryReport, Is.EqualTo("Updated report"));
        Assert.That(updatedContract.Status, Is.EqualTo(ContractStatus.Signed));
    }

    [Test]
    public void ShouldThrowValidationException_WhenIdIsEmpty()
    {
        var command = new UpdateContractCommand
        {
            Id = Guid.Empty,
            ContractDurationInYears = 1,
            ContractSigningDate = DateTimeOffset.Now,
            ImplementationDate = DateTimeOffset.Now.AddMonths(1),
            TotalAccounts = 10,
            Status = ContractStatus.Signed
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenContractDurationInYearsIsZero()
    {
        var command = new UpdateContractCommand
        {
            Id = Guid.NewGuid(),
            ContractDurationInYears = 0,
            ContractSigningDate = DateTimeOffset.Now,
            ImplementationDate = DateTimeOffset.Now.AddMonths(1),
            TotalAccounts = 10,
            Status = ContractStatus.Signed
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenContractSigningDateIsEmpty()
    {
        var command = new UpdateContractCommand
        {
            Id = Guid.NewGuid(),
            ContractDurationInYears = 1,
            ContractSigningDate = default,
            ImplementationDate = DateTimeOffset.Now.AddMonths(1),
            TotalAccounts = 10,
            Status = ContractStatus.Signed
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenImplementationDateIsEmpty()
    {
        var command = new UpdateContractCommand
        {
            Id = Guid.NewGuid(),
            ContractDurationInYears = 1,
            ContractSigningDate = DateTimeOffset.Now,
            ImplementationDate = default,
            TotalAccounts = 10,
            Status = ContractStatus.Signed
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenTotalAccountsIsZero()
    {
        var command = new UpdateContractCommand
        {
            Id = Guid.NewGuid(),
            ContractDurationInYears = 1,
            ContractSigningDate = DateTimeOffset.Now,
            ImplementationDate = DateTimeOffset.Now.AddMonths(1),
            TotalAccounts = 0,
            Status = ContractStatus.Signed
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenStatusIsInvalid()
    {
        var command = new UpdateContractCommand
        {
            Id = Guid.NewGuid(),
            ContractDurationInYears = 1,
            ContractSigningDate = DateTimeOffset.Now,
            ImplementationDate = DateTimeOffset.Now.AddMonths(1),
            TotalAccounts = 10,
            Status = (ContractStatus)999
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}