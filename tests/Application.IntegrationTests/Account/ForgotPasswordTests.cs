using Educar.Backend.Application.Commands.Account.CreateAccount;
using Educar.Backend.Application.Commands.Account.ResetPassword;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using MediatR;
using Moq;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Account;

[TestFixture]
public class ForgotPasswordTests : TestBase
{
    private Domain.Entities.Client _client;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _client = new Domain.Entities.Client("Existing Client");
        Context.Clients.Add(_client);
        Context.SaveChanges();

        MockIdentityServer.Reset();
    }

    [Test]
    public async Task GivenValidRequest_ShouldTriggerPasswordReset()
    {
        // Arrange
        var email = "existing.account@example.com";
        var accountCommand =
            new CreateAccountCommand("Existing Account", email, "123456", _client.Id, UserRole.Admin);
        await SendAsync(accountCommand);

        MockIdentityServer.Setup(s => s.TriggerPasswordReset(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new ForgotPasswordCommand(email);

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.EqualTo(Unit.Value));

        MockIdentityServer.Verify(s => s.TriggerPasswordReset(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public void ShouldThrowValidationException_WhenEmailIsEmpty()
    {
        var command = new ForgotPasswordCommand(string.Empty);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenEmailIsInvalid()
    {
        var command = new ForgotPasswordCommand("invalid-email");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowException_WhenEmailDoesNotExist()
    {
        var command = new ForgotPasswordCommand("non.existent@example.com");

        MockIdentityServer.Setup(s => s.TriggerPasswordReset(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Assert.ThrowsAsync<Exception>(async () => await SendAsync(command));
        MockIdentityServer.Verify(s => s.TriggerPasswordReset(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}