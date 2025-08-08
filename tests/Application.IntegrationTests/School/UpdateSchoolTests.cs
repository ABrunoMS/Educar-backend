using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.School.CreateSchool;
using Educar.Backend.Application.Commands.School.UpdateSchool;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Application.Queries.School;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.School;

[TestFixture]
public class UpdateSchoolTests : TestBase
{
    private Domain.Entities.Client _client;
    private Domain.Entities.Address _address;
    private Domain.Entities.School _school;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _client = new Domain.Entities.Client("Test Client");
        Context.Clients.Add(_client);
        Context.SaveChanges();

        _address = new Domain.Entities.Address("123 Main St", "Test City", "Test State", "12345", "Test Country")
        {
            Lat = 40.712776m,
            Lng = -74.005974m
        };
        Context.Addresses.Add(_address);
        Context.SaveChanges();

        _school = new Domain.Entities.School("Test School")
        {
            Description = "A test school",
            Address = _address,
            Client = _client
        };
        Context.Schools.Add(_school);
        Context.SaveChanges();
    }

    [Test]
    public async Task GivenValidRequest_ShouldUpdateSchool()
    {
        var schoolCommand = new CreateSchoolCommand("Test School", _client.Id)
        {
            Description = "A test school"
        };

        var response = await SendAsync(schoolCommand);

        // Arrange

        var command = new UpdateSchoolCommand
        {
            Id = response.Id,
            Name = "Updated School",
            Description = "Updated description"
        };

        // Act
        await SendAsync(command);

        // Assert
        var updatedSchool = await Context.Schools
            .Include(s => s.Address)
            .FirstOrDefaultAsync(s => s.Id == response.Id);

        Assert.That(updatedSchool, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(updatedSchool.Name, Is.EqualTo("Updated School"));
            Assert.That(updatedSchool.Description, Is.EqualTo("Updated description"));
        });
    }

    [Test]
    public void ShouldThrowValidationException_WhenIdIsEmpty()
    {
        var command = new UpdateSchoolCommand
        {
            Id = Guid.Empty,
            Name = "Updated School",
            Description = "Updated description"
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new UpdateSchoolCommand
        {
            Id = _school.Id,
            Name = "",
            Description = "Updated description"
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowNotFoundException_WhenIdIsInvalid()
    {
        var command = new UpdateSchoolCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated School",
            Description = "Updated description"
        };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task GivenValidRequestWithoutAddress_ShouldUpdateSchool()
    {
        var schoolCommand = new CreateSchoolCommand("Test School", _client.Id)
        {
            Description = "A test school",
        };

        var response = await SendAsync(schoolCommand);

        // Arrange
        const string? expected = "Updated School";
        const string? updatedDescription = "Updated description";
        var command = new UpdateSchoolCommand
        {
            Id = response.Id,
            Name = expected,
            Description = updatedDescription
        };

        // Act
        await SendAsync(command);

        // Assert
        var updatedSchool = await Context.Schools
            .Include(s => s.Address)
            .FirstOrDefaultAsync(s => s.Id == response.Id);

        Assert.That(updatedSchool, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(updatedSchool.Name, Is.EqualTo(expected));
            Assert.That(updatedSchool.Description, Is.EqualTo(updatedDescription));
        });
    }

    [Test]
    public async Task GivenValidRequestWithAddress_ShouldUpdateSchoolWithAddress()
    {
        const string newSt = "456 New St";
        const string newCity = "New City";
        const string newState = "New State";
        const string postalCode = "67890";
        const string newCountry = "New Country";
        const decimal lat = 34.052235m;
        const decimal lng = -118.243683m;

        var schoolCommand = new CreateSchoolCommand("Test School", _client.Id)
        {
            Description = "A test school",
            Address = new Domain.Entities.Address("123 Main St", "Test City", "Test State", "12345", "Test Country")
            {
                Lat = 40.712776m,
                Lng = -74.005974m
            }
        };

        var response = await SendAsync(schoolCommand);

        var query = new GetSchoolQuery() { Id = response.Id };
        var school = await SendAsync(query);

        var command = new UpdateSchoolCommand
        {
            Id = response.Id,
            Name = "Updated School",
            Description = "Updated description",
            Address = new Domain.Entities.Address(newSt, newCity, newState, postalCode, newCountry)
            {
                Id = school!.Address!.Id,
                Lat = lat,
                Lng = lng
            }
        };

        // Act
        await SendAsync(command);

        // Assert
        var updatedSchool = await Context.Schools
            .Include(s => s.Address)
            .FirstOrDefaultAsync(s => s.Id == response.Id);

        Assert.That(updatedSchool, Is.Not.Null);
        Assert.That(updatedSchool.Name, Is.EqualTo("Updated School"));
        Assert.That(updatedSchool.Description, Is.EqualTo("Updated description"));
        Assert.That(updatedSchool.Address?.Street, Is.EqualTo(newSt));
        Assert.That(updatedSchool.Address.City, Is.EqualTo(newCity));
        Assert.That(updatedSchool.Address.State, Is.EqualTo(newState));
        Assert.That(updatedSchool.Address.PostalCode, Is.EqualTo(postalCode));
        Assert.That(updatedSchool.Address.Country, Is.EqualTo(newCountry));
        Assert.That(updatedSchool.Address.Lat, Is.EqualTo(lat));
        Assert.That(updatedSchool.Address.Lng, Is.EqualTo(lng));
    }
}