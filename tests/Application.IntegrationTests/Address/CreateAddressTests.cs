using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Address.CreateAddress;
using Educar.Backend.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Address;

[TestFixture]
public class CreateAddressTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldCreateAddress()
    {
        // Arrange
        var command = new CreateAddressCommand("123 Main St", "Test City", "Test State", "12345", "Test Country")
        {
            Lat = 40.712776m,
            Lng = -74.005974m
        };

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<IdResponseDto>());

        var createdAddress = await Context.Addresses.FirstOrDefaultAsync(a => a.Id == response.Id);

        Assert.That(createdAddress, Is.Not.Null);
        Assert.That(createdAddress.Street, Is.EqualTo("123 Main St"));
        Assert.That(createdAddress.City, Is.EqualTo("Test City"));
        Assert.That(createdAddress.State, Is.EqualTo("Test State"));
        Assert.That(createdAddress.PostalCode, Is.EqualTo("12345"));
        Assert.That(createdAddress.Country, Is.EqualTo("Test Country"));
        Assert.That(createdAddress.Lat, Is.EqualTo(40.712776m));
        Assert.That(createdAddress.Lng, Is.EqualTo(-74.005974m));
    }

    [Test]
    public void ShouldThrowValidationException_WhenStreetIsEmpty()
    {
        var command = new CreateAddressCommand("", "Test City", "Test State", "12345", "Test Country");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenCityIsEmpty()
    {
        var command = new CreateAddressCommand("123 Main St", "", "Test State", "12345", "Test Country");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenStateIsEmpty()
    {
        var command = new CreateAddressCommand("123 Main St", "Test City", "", "12345", "Test Country");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenPostalCodeIsEmpty()
    {
        var command = new CreateAddressCommand("123 Main St", "Test City", "Test State", "", "Test Country");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenCountryIsEmpty()
    {
        var command = new CreateAddressCommand("123 Main St", "Test City", "Test State", "12345", "");

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}