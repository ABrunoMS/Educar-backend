using Educar.Backend.Application.Commands.Client.UpdateClient;
using Educar.Backend.Domain.Entities;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;
using ClientEntity = Educar.Backend.Domain.Entities.Client;

namespace Educar.Backend.Application.IntegrationTests.Client;

[TestFixture]
public class UpdateClientTests : TestBase
{
    [Test]
    public async Task GivenValidRequest_ShouldUpdateClient()
    {
        var clientId = await CreateClientAsAdminAsync();
        var command = new UpdateClientCommand { Id = clientId, Name = "Updated Name", Description = "Updated Desc" };
        
        await SendAsync(command);
        
        var updatedClient = await FindAsync<ClientEntity>(clientId);
        Assert.That(updatedClient, Is.Not.Null);
        Assert.That(updatedClient.Name, Is.EqualTo("Updated Name"));
    }
}