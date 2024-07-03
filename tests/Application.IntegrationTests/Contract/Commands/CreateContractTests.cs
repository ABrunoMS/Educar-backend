using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Contract.CreateAccountType;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Contract.Commands;

[TestFixture]
public class CreateContractTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        _mockContractsDbSet = new Mock<DbSet<Domain.Entities.Contract>>();
        MockContext.Setup(c => c.Contracts).Returns(_mockContractsDbSet.Object);
    }

    private Mock<DbSet<Domain.Entities.Contract>> _mockContractsDbSet;

    [Test]
    public async Task GivenValidRequest_ShouldCreateContract()
    {
        // Arrange
        var command = new CreateContractCommand(1, DateTimeOffset.Now, DateTimeOffset.Now.AddMonths(1), 10,
            ContractStatus.Signed);

        // Act
        var response = await SendAsync(command);

        // Assert
        MockContext.Verify(m => m.Contracts.Add(It.IsAny<Domain.Entities.Contract>()), Times.Once);
        MockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<CreatedResponseDto>());
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