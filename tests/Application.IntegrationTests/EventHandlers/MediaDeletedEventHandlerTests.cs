using Ardalis.GuardClauses;
using Educar.Backend.Application.Common.Extensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.EventHandlers.Media;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Events;
using Moq;
using NUnit.Framework;

namespace Educar.Backend.Application.IntegrationTests.EventHandlers;

[TestFixture]
public class MediaDeletedEventHandlerTests : TestBase
{
    private Mock<IApplicationDbContext> _contextMock;
    private MediaDeletedEventHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new MediaDeletedEventHandler(_contextMock.Object);
    }

    [Test]
    public async Task GivenValidEvent_ShouldCreateMediaLogAndSaveChanges()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var media = new Domain.Entities.Media("Test Media", MediaPurpose.Game, MediaType.Image, true,
            "https://example.com/media", "ObjectName");
        var notification = new MediaDeletedEvent(media, accountId);
        var account = new Domain.Entities.Account("Test Account", "test@example.com", "123456", UserRole.Student);

        _contextMock.Setup(x => x.Accounts.FindAsync(new object[] { accountId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var mediaLogs = new List<Domain.Entities.MediaLog>();
        _contextMock.Setup(x => x.MediaLogs.Add(It.IsAny<Domain.Entities.MediaLog>()))
            .Callback<Domain.Entities.MediaLog>(mediaLog => mediaLogs.Add(mediaLog));

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.Accounts.FindAsync(new object[] { accountId }, It.IsAny<CancellationToken>()),
            Times.Once);
        _contextMock.Verify(x => x.MediaLogs.Add(It.IsAny<Domain.Entities.MediaLog>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.That(mediaLogs, Has.Count.EqualTo(1));
        var createdLog = mediaLogs.First();
        Assert.Multiple(() =>
        {
            Assert.That(createdLog.Account, Is.EqualTo(account));
            Assert.That(createdLog.Media, Is.EqualTo(media));
            Assert.That(createdLog.CurrentState.ToJsonString(), Is.EqualTo(media.ToJsonObject().ToJsonString()));
            Assert.That(createdLog.Action, Is.EqualTo(AuditableAction.Delete));
        });
    }

    [Test]
    public void GivenInvalidAccountId_ShouldThrowNotFoundException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var media = new Domain.Entities.Media("Test Media", MediaPurpose.Game, MediaType.Image, true,
            "https://example.com/media", "ObjectName");
        var notification = new MediaDeletedEvent(media, accountId);

        _contextMock.Setup(x => x.Accounts.FindAsync(new object[] { accountId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Domain.Entities.Account);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(notification, CancellationToken.None));
    }
}