using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Account.CreateAccount;
using Educar.Backend.Application.Commands.Account.DeleteAccount;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Account;

[TestFixture]
public class DeleteAccountTests : TestBase
{
    private Domain.Entities.Client _client;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _client = new Domain.Entities.Client("Existing Client");
        Context.Clients.Add(_client);
        Context.SaveChanges();
    }

    private async Task<Guid> CreateAccount()
    {
        var command = new CreateAccountCommand("Existing Account", "existing.account@example.com", "123456",
            _client.Id, UserRole.Student)
        {
            AverageScore = 100.50m,
            EventAverageScore = 95.75m,
            Stars = 4
        };

        var response = await SendAsync(command);

        return response.Id;
    }

    [Test]
    public async Task GivenValidRequest_ShouldDeleteAccount()
    {
        var accountId = await CreateAccount();
        // Arrange
        var command = new DeleteAccountCommand(accountId);

        // Act
        await SendAsync(command);

        // Assert
        var deletedAccount = await Context.Accounts.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == command.Id);
        Assert.That(deletedAccount, Is.Not.Null);
        Assert.That(deletedAccount.IsDeleted, Is.True);
    }

    [Test]
    public void ShouldThrowNotFoundException_WhenIdIsInvalid()
    {
        // Arrange
        var command = new DeleteAccountCommand(Guid.NewGuid());
        ; // Invalid Id

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }
}