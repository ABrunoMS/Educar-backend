using System.Text.Json.Nodes;
using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.MediaLog.CreateMediaLog;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.MediaLog;

[TestFixture]
public class CreateMediaLogCommandTests : TestBase
{
    private Domain.Entities.Account _account;
    private Domain.Entities.Media _media;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _account = new Domain.Entities.Account("Test Account", "email@test.com", "000", UserRole.Admin);
        Context.Accounts.Add(_account);
        Context.SaveChanges();

        _media = new Domain.Entities.Media("Test Media", MediaPurpose.Game, MediaType.Image, true, "url", "object.png");
        Context.Medias.Add(_media);
        Context.SaveChanges();
    }

    [Test]
    public async Task GivenValidRequest_ShouldCreateMediaLog()
    {
        // Arrange
        var command = new CreateMediaLogCommand(AuditableAction.Create, new JsonObject(), _account.Id, _media.Id);

        // Act
        var response = await SendAsync(command);

        // Assert
        var createdMediaLog = await Context.MediaLogs.FindAsync(response.Id);
        Assert.That(createdMediaLog, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createdMediaLog.Action, Is.EqualTo(AuditableAction.Create));
            Assert.That(createdMediaLog.CurrentState, Is.EqualTo(new JsonObject()));
            Assert.That(createdMediaLog.AccountId, Is.EqualTo(_account.Id));
            Assert.That(createdMediaLog.MediaId, Is.EqualTo(_media.Id));
            Assert.That(createdMediaLog.PreviousState, Is.Null);
        });
    }

    [Test]
    public void GivenInvalidAccountId_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new CreateMediaLogCommand(AuditableAction.Create, new JsonObject(), Guid.NewGuid(), _media.Id);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }

    [Test]
    public void GivenInvalidMediaId_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new CreateMediaLogCommand(AuditableAction.Create, new JsonObject(), _account.Id, Guid.NewGuid());

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }

    [Test]
    public void GivenInvalidCommand_ShouldThrowValidationException()
    {
        // Arrange
        var command = new CreateMediaLogCommand(AuditableAction.None, new JsonObject(), Guid.Empty, Guid.Empty);

        // Act & Assert
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void GivenUpdateActionWithNoPreviousState_ShouldThrowValidationException()
    {
        // Arrange
        var command = new CreateMediaLogCommand(AuditableAction.Update, new JsonObject(), _account.Id, _media.Id);

        // Act & Assert
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task GivenUpdateActionWithPreviousState_ShouldCreateMediaLog()
    {
        // Arrange
        var command = new CreateMediaLogCommand(AuditableAction.Update, new JsonObject(), _account.Id, _media.Id)
        {
            PreviousState = new JsonObject()
        };

        // Act
        var response = await SendAsync(command);

        // Assert
        var createdMediaLog = await Context.MediaLogs.FindAsync(response.Id);
        Assert.That(createdMediaLog, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createdMediaLog.Action, Is.EqualTo(AuditableAction.Update));
            Assert.That(createdMediaLog.CurrentState, Is.EqualTo(new JsonObject()));
            Assert.That(createdMediaLog.AccountId, Is.EqualTo(_account.Id));
            Assert.That(createdMediaLog.MediaId, Is.EqualTo(_media.Id));
            Assert.That(createdMediaLog.PreviousState, Is.EqualTo(new JsonObject()));
        });
    }
}
