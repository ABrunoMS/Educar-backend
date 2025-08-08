using Educar.Backend.Application.Queries.Client;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Client;

[TestFixture]
public class GetClientTests : TestBase
{
    [Test]
    public async Task GivenValidId_ShouldReturnClient()
    {
        var clientId = await CreateClientAsAdminAsync("Cliente para Buscar");
        var clientDto = await SendAsync(new GetClientQuery { Id = clientId });

        Assert.That(clientDto, Is.Not.Null);
        Assert.That(clientDto.Name, Is.EqualTo("Cliente para Buscar"));
        Assert.That(clientDto.Contract, Is.Not.Null);
    }
}