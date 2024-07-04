using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Contract.CreateAccountType;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Contract.Commands;

[TestFixture]
public class CreateContractTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldCreateContract()
    {
        // Arrange
        var command = new CreateContractCommand(1, DateTimeOffset.Now, DateTimeOffset.Now.AddMonths(1), 10,
            ContractStatus.Signed);

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<CreatedResponseDto>());

        var createdContract = await Context.Contracts.FindAsync(response.Id);
        Assert.That(createdContract, Is.Not.Null);
        Assert.That(createdContract.Id, Is.Not.Empty);
    }

    [Test]
    public void ShouldThrowValidationException_WhenContractDurationInYearsIsZero()
    {
        var command = new CreateContractCommand(0, DateTimeOffset.Now, DateTimeOffset.Now.AddMonths(1), 10,
            ContractStatus.Signed);
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenContractSigningDateIsEmpty()
    {
        var command = new CreateContractCommand(1, default, DateTimeOffset.Now.AddMonths(1), 10, ContractStatus.Signed);
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenImplementationDateIsEmpty()
    {
        var command = new CreateContractCommand(1, DateTimeOffset.Now, default, 10, ContractStatus.Signed);
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenTotalAccountsIsZero()
    {
        var command = new CreateContractCommand(1, DateTimeOffset.Now, DateTimeOffset.Now.AddMonths(1), 0,
            ContractStatus.Signed);
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenStatusIsInvalid()
    {
        var command = new CreateContractCommand(1, DateTimeOffset.Now, DateTimeOffset.Now.AddMonths(1), 10,
            (ContractStatus)999);
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}