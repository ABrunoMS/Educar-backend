using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Address.DeleteAddress;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Address;

[TestFixture]
public class DeleteAddressTests : TestBase
{
    private Domain.Entities.Address _address;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _address = new Domain.Entities.Address("123 Main St", "Test City", "Test State", "12345", "Test Country")
        {
            Lat = 40.712776m,
            Lng = -74.005974m
        };
        Context.Addresses.Add(_address);
        Context.SaveChanges();
    }

    [Test]
    public async Task GivenValidId_ShouldDeleteAddress()
    {
        // Arrange
        var command = new DeleteAddressCommand(_address.Id);

        // Act
        await SendAsync(command);

        // Assert
        var deletedAddress = await Context.Addresses.FirstOrDefaultAsync(a => a.Id == _address.Id);
        Assert.That(deletedAddress, Is.Null);
    }

    [Test]
    public void ShouldThrowNotFoundException_WhenIdIsInvalid()
    {
        // Arrange
        var command = new DeleteAddressCommand(Guid.NewGuid());

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenIdIsEmpty()
    {
        // Arrange
        var command = new DeleteAddressCommand(Guid.Empty);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }
}