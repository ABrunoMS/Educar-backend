using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Map.CreateMap;
using Educar.Backend.Application.Queries.Map;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Map;

[TestFixture]
public class GetMapQueryTests : TestBase
{
    private Domain.Entities.Game _game;

    [SetUp]
    public void SetUp()
    {
        ResetState();

        _game = new Domain.Entities.Game("Test Game", "Game Description", "Lore", "Purpose");
        Context.Games.Add(_game);
        Context.SaveChanges();
    }

    [Test]
    public async Task GivenValidRequest_ShouldReturnMap()
    {
        var mapId = await CreateMap("Test Map", "Test Description", MapType.Dungeon, "Reference2D", "Reference3D");

        // Arrange
        var query = new GetMapQuery { Id = mapId };

        // Act
        var queryResponse = await SendAsync(query);

        // Assert
        Assert.That(queryResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(queryResponse.Id, Is.EqualTo(mapId));
            Assert.That(queryResponse.Name, Is.EqualTo("Test Map"));
            Assert.That(queryResponse.Description, Is.EqualTo("Test Description"));
        });
    }

    [Test]
    public void GivenInvalidRequest_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetMapQuery { Id = Guid.NewGuid() };

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenValidPaginationRequest_ShouldReturnPaginatedMapsByGame()
    {
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            await CreateMap($"Map {i}", $"Description {i}", MapType.Dungeon, "Reference2D", "Reference3D");
        }

        var query = new GetMapByGamePaginatedQuery(_game.Id) { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(10));
            Assert.That(result.PageNumber, Is.EqualTo(1));
            Assert.That(result.TotalCount, Is.EqualTo(20));
            Assert.That(result.TotalPages, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GivenSpecificPageRequest_ShouldReturnCorrectPageByGame()
    {
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            await CreateMap($"Map {i}", $"Description {i}", MapType.Dungeon, "Reference2D", "Reference3D");
        }

        var query = new GetMapByGamePaginatedQuery(_game.Id) { PageNumber = 2, PageSize = 10 };

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
        // Arrange
        for (var i = 1; i <= 20; i++)
        {
            await CreateMap($"Map {i}", $"Description {i}", MapType.Dungeon, "Reference2D", "Reference3D");
        }

        var query = new GetMapByGamePaginatedQuery(_game.Id) { PageNumber = 3, PageSize = 10 };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Is.Empty);
            Assert.That(result.PageNumber, Is.EqualTo(3));
            Assert.That(result.TotalCount, Is.EqualTo(20));
            Assert.That(result.TotalPages, Is.EqualTo(2));
        });
    }

    private async Task<Guid> CreateMap(string name, string description, MapType type, string reference2D,
        string reference3D)
    {
        var mapCommand = new CreateMapCommand(name, description, type, reference2D, reference3D, _game.Id);
        var response = await SendAsync(mapCommand);
        return response.Id;
    }
}