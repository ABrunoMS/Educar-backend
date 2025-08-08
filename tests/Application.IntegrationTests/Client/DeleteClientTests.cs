using Educar.Backend.Application.Commands.Client.DeleteClient;
using Educar.Backend.Domain.Entities;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;
using ClientEntity = Educar.Backend.Domain.Entities.Client;

namespace Educar.Backend.Application.IntegrationTests.Client;

[TestFixture]
public class DeleteClientTests : TestBase
{
    [Test]
    public async Task GivenValidId_ShouldDeleteClient()
    {
        var clientId = await CreateClientAsAdminAsync();
        await SendAsync(new DeleteClientCommand(clientId));
        var client = await FindAsync<ClientEntity>(clientId);
        Assert.That(client, Is.Null);
    }
}