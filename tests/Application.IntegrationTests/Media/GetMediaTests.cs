using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Media.CreateMedia;
using Educar.Backend.Application.Queries.Media;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Media;

[TestFixture]
public class GetMediaTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidId_ShouldReturnMedia()
    {
        // Arrange
        var command = new CreateMediaCommand("Test Media", "ObjectName", "https://example.com/media", MediaPurpose.Game,
            MediaType.Image, true)
        {
            References = "Some references",
            Author = "Author Name"
        };
        var createdResponse = await SendAsync(command);
        var mediaId = createdResponse.Id;

        var query = new GetMediaQuery { Id = mediaId };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Test Media"));
        Assert.That(result.ObjectName, Is.EqualTo("ObjectName"));
        Assert.That(result.Url, Is.EqualTo("https://example.com/media"));
        Assert.That(result.Purpose, Is.EqualTo(MediaPurpose.Game));
        Assert.That(result.Type, Is.EqualTo(MediaType.Image));
        Assert.That(result.References, Is.EqualTo("Some references"));
        Assert.That(result.Author, Is.EqualTo("Author Name"));
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        var query = new GetMediaQuery { Id = Guid.NewGuid() };

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public async Task GivenBothPurposeAndType_ShouldReturnFilteredMedia()
    {
        // Arrange
        var command1 = new CreateMediaCommand("Media 1", "ObjectName1", "https://example.com/media1", MediaPurpose.Game,
            MediaType.Image, true);
        var command2 = new CreateMediaCommand("Media 2", "ObjectName2", "https://example.com/media2", MediaPurpose.Game,
            MediaType.Video, true);
        var command3 = new CreateMediaCommand("Media 3", "ObjectName3", "https://example.com/media3",
            MediaPurpose.Quest, MediaType.Image, true);
        await SendAsync(command1);
        await SendAsync(command2);
        await SendAsync(command3);

        var query = new GetMediaByPurposeAndTypePaginatedQuery
        {
            Purpose = MediaPurpose.Game,
            Type = MediaType.Image,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items, Has.Count.EqualTo(1));
        Assert.That(result.Items.First().Name, Is.EqualTo("Media 1"));
    }

    [Test]
    public async Task GivenOnlyPurpose_ShouldReturnFilteredMedia()
    {
        // Arrange
        var command1 = new CreateMediaCommand("Media 1", "ObjectName1", "https://example.com/media1", MediaPurpose.Game,
            MediaType.Image, true);
        var command2 = new CreateMediaCommand("Media 2", "ObjectName2", "https://example.com/media2", MediaPurpose.Game,
            MediaType.Video, true);
        var command3 = new CreateMediaCommand("Media 3", "ObjectName3", "https://example.com/media3",
            MediaPurpose.Quest,
            MediaType.Image, true);
        await SendAsync(command1);
        await SendAsync(command2);
        await SendAsync(command3);

        var query = new GetMediaByPurposeAndTypePaginatedQuery
        {
            Purpose = MediaPurpose.Game,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GivenOnlyType_ShouldReturnFilteredMedia()
    {
        // Arrange
        var command1 = new CreateMediaCommand("Media 1", "ObjectName1", "https://example.com/media1", MediaPurpose.Game,
            MediaType.Image, true);
        var command2 = new CreateMediaCommand("Media 2", "ObjectName2", "https://example.com/media2", MediaPurpose.Game,
            MediaType.Video, true);
        var command3 = new CreateMediaCommand("Media 3", "ObjectName3", "https://example.com/media3",
            MediaPurpose.Quest,
            MediaType.Image, true);
        await SendAsync(command1);
        await SendAsync(command2);
        await SendAsync(command3);

        var query = new GetMediaByPurposeAndTypePaginatedQuery
        {
            Type = MediaType.Image,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GivenNeitherPurposeNorType_ShouldReturnAllMedia()
    {
        // Arrange
        var command1 = new CreateMediaCommand("Media 1", "ObjectName1", "https://example.com/media1", MediaPurpose.Game,
            MediaType.Image, true);
        var command2 = new CreateMediaCommand("Media 2", "ObjectName2", "https://example.com/media2", MediaPurpose.Game,
            MediaType.Video, true);
        var command3 = new CreateMediaCommand("Media 3", "ObjectName3", "https://example.com/media3",
            MediaPurpose.Quest,
            MediaType.Image, true);
        await SendAsync(command1);
        await SendAsync(command2);
        await SendAsync(command3);

        var query = new GetMediaByPurposeAndTypePaginatedQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items, Has.Count.EqualTo(3));
    }
}