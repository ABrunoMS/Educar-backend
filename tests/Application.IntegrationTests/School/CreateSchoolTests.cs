using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.School.CreateSchool;
using Educar.Backend.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.School;

[TestFixture]
public class CreateSchoolTests : TestBase
{
    private Domain.Entities.Client _client;
    private Domain.Entities.Address _address;

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
    }

    [Test]
    public async Task GivenValidRequest_ShouldCreateSchool()
    {
        // Arrange
        var command = new CreateSchoolCommand("Test School", _client.Id)
        {
            Description = "A test school",
            Address = _address
        };

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<IdResponseDto>());

        var createdSchool = await Context.Schools
            .Include(s => s.Client)
            .FirstOrDefaultAsync(s => s.Id == response.Id);

        Assert.That(createdSchool, Is.Not.Null);
        Assert.That(createdSchool.Name, Is.EqualTo("Test School"));
        Assert.That(createdSchool.Description, Is.EqualTo("A test school"));
        Assert.That(createdSchool.Client!.Id, Is.EqualTo(_client.Id));
    }

    [Test]
    public async Task GivenValidRequestWithoutAddress_ShouldCreateSchool()
    {
        // Arrange
        var command = new CreateSchoolCommand("Test School", _client.Id)
        {
            Description = "A test school"
        };

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<IdResponseDto>());

        var createdSchool = await Context.Schools
            .Include(s => s.Client)
            .FirstOrDefaultAsync(s => s.Id == response.Id);

        Assert.That(createdSchool, Is.Not.Null);
        Assert.That(createdSchool.Name, Is.EqualTo("Test School"));
        Assert.That(createdSchool.Description, Is.EqualTo("A test school"));
        Assert.That(createdSchool.Client!.Id, Is.EqualTo(_client.Id));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new CreateSchoolCommand("", _client.Id)
        {
            Description = "A test school",
            Address = _address
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenClientIdIsEmpty()
    {
        var command = new CreateSchoolCommand("Test School", Guid.Empty)
        {
            Description = "A test school",
            Address = _address
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowNotFoundException_WhenClientIdIsInvalid()
    {
        var command = new CreateSchoolCommand("Test School", Guid.NewGuid())
        {
            Description = "A test school",
            Address = _address
        };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task GivenValidRequestWithAddress_ShouldCreateSchoolWithAddress()
    {
        // Arrange
        var command = new CreateSchoolCommand("Test School", _client.Id)
        {
            Description = "A test school",
            Address = new Domain.Entities.Address("456 Secondary St", "Another City", "Another State", "67890",
                "Another Country")
            {
                Lat = 37.774929m,
                Lng = -122.419416m
            }
        };

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<IdResponseDto>());

        var createdSchool = await Context.Schools
            .Include(s => s.Client)
            .Include(s => s.Address)
            .FirstOrDefaultAsync(s => s.Id == response.Id);

        Assert.That(createdSchool, Is.Not.Null);
        Assert.That(createdSchool.Name, Is.EqualTo("Test School"));
        Assert.That(createdSchool.Description, Is.EqualTo("A test school"));
        Assert.That(createdSchool.Client!.Id, Is.EqualTo(_client.Id));
        Assert.That(createdSchool.Address, Is.Not.Null);
        Assert.That(createdSchool.Address.Street, Is.EqualTo("456 Secondary St"));
        Assert.That(createdSchool.Address.City, Is.EqualTo("Another City"));
        Assert.That(createdSchool.Address.State, Is.EqualTo("Another State"));
        Assert.That(createdSchool.Address.PostalCode, Is.EqualTo("67890"));
        Assert.That(createdSchool.Address.Country, Is.EqualTo("Another Country"));
        Assert.That(createdSchool.Address.Lat, Is.EqualTo(37.774929m));
        Assert.That(createdSchool.Address.Lng, Is.EqualTo(-122.419416m));
    }
}