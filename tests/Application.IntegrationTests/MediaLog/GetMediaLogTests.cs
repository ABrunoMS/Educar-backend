using System.Text.Json.Nodes;
using Educar.Backend.Application.Commands.Account.CreateAccount;
using Educar.Backend.Application.Commands.MediaLog.CreateMediaLog;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Queries.MediaLog;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.MediaLog;

[TestFixture]
public class GetMediaLogTests
{
    private Domain.Entities.Client _client;
    private Domain.Entities.Media _media;
    private Domain.Entities.Account _account;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _client = new Domain.Entities.Client("Test Client");
        Context.Clients.Add(_client);
        Context.SaveChanges();

        _media = new Domain.Entities.Media("Test Media", MediaPurpose.Game, MediaType.Image, true, "url", "object.png");
        Context.Medias.Add(_media);
        Context.SaveChanges();

        _account = new Domain.Entities.Account("Test Account", "test@email.com", "000", UserRole.Admin)
        {
            ClientId = _client.Id
        };
        Context.Accounts.Add(_account);
        Context.SaveChanges();
    }

    [Test]
    public async Task GivenValidMediaId_ShouldReturnPaginatedMediaLogs()
    {
        await CreateMediaLogs();
        
        // Arrange
        var query = new GetMediaLogByMediaIdPaginatedQuery(_media.Id) { PageNumber = 1, PageSize = 10 };
        
        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items.Count, Is.EqualTo(10));
        Assert.That(result.PageNumber, Is.EqualTo(1));
        Assert.That(result.TotalCount, Is.EqualTo(20));
        Assert.That(result.TotalPages, Is.EqualTo(2));
    }

    [Test]
    public async Task GivenSpecificPageRequest_ShouldReturnCorrectPage()
    {
        await CreateMediaLogs();
        
        // Arrange
        var query = new GetMediaLogByMediaIdPaginatedQuery(_media.Id) { PageNumber = 2, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(10));
            Assert.That(result.PageNumber, Is.EqualTo(2));
            Assert.That(result.TotalCount, Is.EqualTo(20));
            Assert.That(result.TotalPages, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GivenOutOfRangePageRequest_ShouldReturnEmptyPage()
    {
        await CreateMediaLogs();

        var query = new GetMediaLogByMediaIdPaginatedQuery(_media.Id) { PageNumber = 3, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(0));
            Assert.That(result.PageNumber, Is.EqualTo(3));
            Assert.That(result.TotalCount, Is.EqualTo(20));
            Assert.That(result.TotalPages, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GivenInvalidMediaId_ShouldReturnEmpty()
    {
        // Arrange
        var query = new GetMediaLogByMediaIdPaginatedQuery(Guid.NewGuid());

        var result = await SendAsync(query);
        // Act & Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items, Is.Empty);
    }

    [Test]
    public async Task GivenNoMediaLogs_ShouldReturnEmptyPage()
    {
        var query = new GetMediaLogByMediaIdPaginatedQuery(_media.Id) { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(0));
            Assert.That(result.PageNumber, Is.EqualTo(1));
            Assert.That(result.TotalCount, Is.EqualTo(0));
            Assert.That(result.TotalPages, Is.EqualTo(0));
        });
    }

    private async Task CreateMediaLogs()
    {
        // Arrange
        for (var i = 0; i < 20; i++)
        {
            var command = new CreateMediaLogCommand(AuditableAction.Create, new JsonObject(), _account.Id, _media.Id);
            await SendAsync(command);
        }
    }
}