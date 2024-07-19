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
}