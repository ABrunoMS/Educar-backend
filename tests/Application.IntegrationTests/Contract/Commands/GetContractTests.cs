using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Contract.CreateAccountType;
using Educar.Backend.Application.Queries.Contract;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Contract.Commands;

[TestFixture]
public class GetContractTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldReturnContract()
    {
        // Arrange
        var command = new CreateContractCommand(1, DateTimeOffset.Now, DateTimeOffset.Now.AddMonths(1), 10,
            ContractStatus.Signed);
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
}