using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Client.CreateClient;
using Educar.Backend.Application.Commands.Client.UpdateClient;
using Educar.Backend.Application.Common.Exceptions;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Client;

[TestFixture]
public class UpdateClientTests : TestBase
{
    private const string ValidUpdatedName = "Updated Client";
    private const string ValidUpdatedDescription = "Updated Description";
    private const string InitialClientName = "Initial Client";
    private const string InitialClientDescription = "Initial Description";

    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldUpdateClient()
    {
        // Arrange
        var createCommand = new CreateClientCommand(InitialClientName)
        {
            Description = InitialClientDescription
        };
        var createResponse = await SendAsync(createCommand);

        var updateCommand = new UpdateClientCommand(ValidUpdatedName, ValidUpdatedDescription)
        {
            Id = createResponse.Id
        };

        // Act
        await SendAsync(updateCommand);

        // Assert
        var updatedClient = await Context.Clients.FindAsync(updateCommand.Id);
        Assert.That(updatedClient, Is.Not.Null);
        Assert.That(updatedClient.Name, Is.EqualTo(ValidUpdatedName));
        Assert.That(updatedClient.Description, Is.EqualTo(ValidUpdatedDescription));
    }

    [Test]
    public void ShouldThrowValidationException_WhenIdIsEmpty()
    {
        var command = new UpdateClientCommand(ValidUpdatedName, ValidUpdatedDescription)
        {
            Id = Guid.Empty
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new UpdateClientCommand(string.Empty, ValidUpdatedDescription)
        {
            Id = Guid.NewGuid()
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        var command = new UpdateClientCommand(ValidUpdatedName, ValidUpdatedDescription)
        {
            Id = Guid.NewGuid()
        };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }
}

