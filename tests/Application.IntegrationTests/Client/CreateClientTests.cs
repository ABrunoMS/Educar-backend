using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.AccountType.CreateAccountType;
using Educar.Backend.Application.Common.Exceptions;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Client;

[TestFixture]
public class CreateClientTests: TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldCreateClient()
    {
        // Arrange
        var command = new CreateClientCommand("New Client")
        {
            Description = "Client Description"
        };

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<CreatedResponseDto>());

        if (Context.Clients != null)
        {
            var createdClient = await Context.Clients.FindAsync(response.Id);
            Assert.That(createdClient, Is.Not.Null);
            Assert.That(createdClient.Name, Is.EqualTo("New Client"));
            Assert.That(createdClient.Description, Is.EqualTo("Client Description"));
            Assert.That(createdClient.Contract, Is.Null);
        }
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new CreateClientCommand(string.Empty)
        {
            Description = "Client Description"
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task GivenValidRequestWithoutDescription_ShouldCreateClient()
    {
        // Arrange
        var command = new CreateClientCommand("Client Without Description");

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<CreatedResponseDto>());

        var createdClient = await Context.Clients.FindAsync(response.Id);
        Assert.That(createdClient, Is.Not.Null);
        Assert.That(createdClient.Name, Is.EqualTo("Client Without Description"));
        Assert.That(createdClient.Description, Is.Null);
    }
}