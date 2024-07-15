using Educar.Backend.Application.Commands.Address.CreateAddress;
using Educar.Backend.Application.Commands.Address.UpdateAddress;
using Educar.Backend.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Address;

[TestFixture]
public class UpdateAddressTests : TestBase
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
    public async Task GivenValidRequest_ShouldUpdateAddress()
    {
        var createCommand = new CreateAddressCommand("street", "city", "state", "postalCode", "country");
        var response = await SendAsync(createCommand);

        // Arrange
        const string? newSt = "456 New St";
        const string? newCity = "New City";
        const string? newState = "New State";
        const string? postalCode = "67890";
        const string? newCountry = "New Country";
        const decimal expected = 34.052235m;
        const decimal lng = -118.243683m;
        var command = new UpdateAddressCommand
        {
            Id = response.Id,
            Street = newSt,
            City = newCity,
            State = newState,
            PostalCode = postalCode,
            Country = newCountry,
            Lat = expected,
            Lng = lng
        };

        // Act
        await SendAsync(command);

        // Assert
        var updatedAddress = await Context.Addresses.FirstOrDefaultAsync(a => a.Id == response.Id);

        Assert.That(updatedAddress, Is.Not.Null);
        Assert.That(updatedAddress.Street, Is.EqualTo(newSt));
        Assert.That(updatedAddress.City, Is.EqualTo(newCity));
        Assert.That(updatedAddress.State, Is.EqualTo(newState));
        Assert.That(updatedAddress.PostalCode, Is.EqualTo(postalCode));
        Assert.That(updatedAddress.Country, Is.EqualTo(newCountry));
        Assert.That(updatedAddress.Lat, Is.EqualTo(expected));
        Assert.That(updatedAddress.Lng, Is.EqualTo(lng));
    }

    [Test]
    public void ShouldThrowValidationException_WhenIdIsEmpty()
    {
        var command = new UpdateAddressCommand
        {
            Id = Guid.Empty,
            Street = "456 New St",
            City = "New City",
            State = "New State",
            PostalCode = "67890",
            Country = "New Country",
            Lat = 34.052235m,
            Lng = -118.243683m
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenStreetIsEmpty()
    {
        var command = new UpdateAddressCommand
        {
            Id = _address.Id,
            Street = "",
            City = "New City",
            State = "New State",
            PostalCode = "67890",
            Country = "New Country",
            Lat = 34.052235m,
            Lng = -118.243683m
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenCityIsEmpty()
    {
        var command = new UpdateAddressCommand
        {
            Id = _address.Id,
            Street = "456 New St",
            City = "",
            State = "New State",
            PostalCode = "67890",
            Country = "New Country",
            Lat = 34.052235m,
            Lng = -118.243683m
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenStateIsEmpty()
    {
        var command = new UpdateAddressCommand
        {
            Id = _address.Id,
            Street = "456 New St",
            City = "New City",
            State = "",
            PostalCode = "67890",
            Country = "New Country",
            Lat = 34.052235m,
            Lng = -118.243683m
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenPostalCodeIsEmpty()
    {
        var command = new UpdateAddressCommand
        {
            Id = _address.Id,
            Street = "456 New St",
            City = "New City",
            State = "New State",
            PostalCode = "",
            Country = "New Country",
            Lat = 34.052235m,
            Lng = -118.243683m
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenCountryIsEmpty()
    {
        var command = new UpdateAddressCommand
        {
            Id = _address.Id,
            Street = "456 New St",
            City = "New City",
            State = "New State",
            PostalCode = "67890",
            Country = "",
            Lat = 34.052235m,
            Lng = -118.243683m
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}