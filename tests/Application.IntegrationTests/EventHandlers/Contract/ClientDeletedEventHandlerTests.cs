using Educar.Backend.Application.EventHandlers.Client;
using Educar.Backend.Application.Interfaces;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Educar.Backend.Application.IntegrationTests.EventHandlers.Contract;

[TestFixture]
public class ClientDeletedEventHandlerTests
{
    private Mock<IApplicationDbContext> _contextMock;
    private Mock<ILogger<ClientDeletedEventHandler>> _loggerMock;
    private ClientDeletedEventHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _loggerMock = new Mock<ILogger<ClientDeletedEventHandler>>();
        _handler = new ClientDeletedEventHandler(_contextMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task ShouldRemoveContractAndLog_WhenClientHasContract()
    {
        // Arrange
        var contract = new Domain.Entities.Contract(1, DateTimeOffset.Now, DateTimeOffset.Now.AddMonths(1), 10,
            ContractStatus.Signed)
        {
            Id = Guid.NewGuid()
        };
        var client = new Domain.Entities.Client("Test Client") { Contract = contract };
        var notification = new ClientDeletedEvent(client);

        // Mock the Contracts DbSet
        var contractsMock = new Mock<DbSet<Domain.Entities.Contract>>();
        _contextMock.Setup(c => c.Contracts).Returns(contractsMock.Object);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _contextMock.Verify(c => c.Contracts.Remove(It.Is<Domain.Entities.Contract>(con => con == contract)), Times.Once);
    }

    [Test]
    public async Task ShouldNotRemoveContractAndNotLog_WhenClientHasNoContract()
    {
        // Arrange
        var client = new Domain.Entities.Client("Test Client");
        var notification = new ClientDeletedEvent(client);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _contextMock.Verify(c => c.Contracts.Remove(It.IsAny<Domain.Entities.Contract>()), Times.Never);
    }
}
