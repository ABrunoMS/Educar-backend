using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.School.CreateSchool;
using Educar.Backend.Application.Commands.School.DeleteSchool;
using Educar.Backend.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.School;

[TestFixture]
public class DeleteSchoolTests : TestBase
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
    public async Task GivenValidId_ShouldDeleteSchool()
    {
        var createCommand = new CreateSchoolCommand("name", _client.Id);
        var response = await SendAsync(createCommand);

        // Arrange
        var command = new DeleteSchoolCommand(response.Id);

        // Act
        await SendAsync(command);

        // Assert
        var deletedSchool = await Context.Schools.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == response.Id);
        Assert.That(deletedSchool, Is.Not.Null);
        Assert.That(deletedSchool.IsDeleted, Is.True);
    }

    public async Task GivenValidId_ShouldDeleteSchoolAndAddress()
    {
        var createCommand = new CreateSchoolCommand("name", _client.Id)
        {
            Address = new Domain.Entities.Address("street", "city", "state", "zip", "country")
        };
        var response = await SendAsync(createCommand);

        // Arrange
        var command = new DeleteSchoolCommand(response.Id);

        // Act
        await SendAsync(command);

        // Assert
        var deletedSchool = await Context.Schools
            .IgnoreQueryFilters()
            .Include(school => school.Address)
            .FirstOrDefaultAsync(s => s.Id == response.Id);

        Assert.That(deletedSchool, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(deletedSchool.IsDeleted, Is.True);
            Assert.That(deletedSchool.Address, Is.Not.Null);
        });
        Assert.That(deletedSchool.Address.IsDeleted, Is.True);
    }

    [Test]
    public void ShouldThrowNotFoundException_WhenIdIsInvalid()
    {
        // Arrange
        var command = new DeleteSchoolCommand(Guid.NewGuid());

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenIdIsEmpty()
    {
        // Arrange
        var command = new DeleteSchoolCommand(Guid.Empty);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }

    [Test]
    public async Task GivenDeletedSchoolRequest_ShouldThrowNotFoundException()
    {
        // Arrange
        Context.Schools.Remove(_school);
        await Context.SaveChangesAsync();

        var command = new DeleteSchoolCommand(_school.Id);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(command));
    }
}