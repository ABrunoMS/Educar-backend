using Educar.Backend.Application.Commands.Client.CreateClient;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Entities;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;
using ClientEntity = Educar.Backend.Domain.Entities.Client;

namespace Educar.Backend.Application.IntegrationTests.Client;

[TestFixture]
public class CreateClientTests : TestBase
{
    [Test]
    public async Task GivenValidRequest_ShouldCreateClient()
    {
        var clientId = await CreateClientAsAdminAsync("New Client");
        var createdClient = await FindAsync<ClientEntity>(clientId);

        Assert.That(createdClient, Is.Not.Null);
        Assert.That(createdClient.Name, Is.EqualTo("New Client"));
        Assert.That(createdClient.Contract, Is.EqualTo("Contrato de Teste"));
    }
}