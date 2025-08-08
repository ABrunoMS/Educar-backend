using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.EventHandlers.Account;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Events;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Educar.Backend.Application.IntegrationTests.EventHandlers;

[TestFixture]
public class AccountCreatedEventHandlerTests : TestBase
{
    private Mock<ILogger<AccountCreatedEventHandler>> _loggerMock;
    private Mock<IIdentityService> _identityServiceMock;
    private AccountCreatedEventHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<AccountCreatedEventHandler>>();
        _identityServiceMock = new Mock<IIdentityService>();
        _handler = new AccountCreatedEventHandler(_loggerMock.Object, _identityServiceMock.Object);
    }

    [Test]
    public async Task GivenValidEvent_ShouldCreateUserAndLogInformation()
    {
        // Arrange
        var account = new Domain.Entities.Account("Test Account", "test@example.com", "123456", UserRole.Student);
        var notification = new AccountCreatedEvent(account);
        var expectedUserId = Guid.NewGuid();

        _identityServiceMock
            .Setup(x => x.CreateUser(account.Email, account.Name, account.Role, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserId);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _identityServiceMock.Verify(
            x => x.CreateUser(account.Email, account.Name, account.Role, It.IsAny<CancellationToken>()), Times.Once);
        Assert.That(account.Id, Is.EqualTo(expectedUserId));
    }

    [Test]
    public void GivenFailedUserCreation_ShouldThrowException()
    {
        // Arrange
        var account = new Domain.Entities.Account("Test Account", "test@example.com", "123456", UserRole.Student);
        var notification = new AccountCreatedEvent(account);

        _identityServiceMock
            .Setup(x => x.CreateUser(account.Email, account.Name, account.Role, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty);

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(() => _handler.Handle(notification, CancellationToken.None));
        Assert.That(exception.Message, Is.EqualTo("Failed to create user"));
    }

    [Test]
    public void GivenExceptionInUserCreation_ShouldLogError()
    {
        // Arrange
        var account = new Domain.Entities.Account("Test Account", "test@example.com", "123456", UserRole.Student);
        var notification = new AccountCreatedEvent(account);

        _identityServiceMock
            .Setup(x => x.CreateUser(account.Email, account.Name, account.Role, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Identity service error"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(() => _handler.Handle(notification, CancellationToken.None));
    }
}