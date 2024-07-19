using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Media.CreateMedia;
using Educar.Backend.Application.Commands.Media.DeleteMedia;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Queries.Media;
using Educar.Backend.Domain.Enums;
using Moq;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Media;

[TestFixture]
public class DeleteMediaTests : TestBase
{
    private Mock<IObjectStorage> _mockObjectStorage;

    [SetUp]
    public void SetUp()
    {
        ResetState();
        _mockObjectStorage = MockObjectStorage;
    }

    [Test]
    public async Task GivenValidId_ShouldDeleteMedia()
    {
        // Arrange
        var createCommand = new CreateMediaCommand("Test Media", "ObjectName", "https://example.com/media",
            MediaPurpose.Game, MediaType.Image, true)
        {
            References = "References",
            Author = "Author"
        };
        var createdResponse = await SendAsync(createCommand);
        var mediaId = createdResponse.Id;

        var deleteCommand = new DeleteMediaCommand(mediaId);

        // Mock the behavior of DeleteObjectAsync to return true
        _mockObjectStorage.Setup(s => s.DeleteObjectAsync("ObjectName", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await SendAsync(deleteCommand);

        // Assert
        var query = new GetMediaQuery { Id = mediaId };
        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(query));
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        var deleteCommand = new DeleteMediaCommand(Guid.NewGuid());

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(deleteCommand));
    }

    [Test]
    public async Task GivenFailureToDeleteObject_ShouldThrowException()
    {
        // Arrange
        var createCommand = new CreateMediaCommand("Test Media", "ObjectName", "https://example.com/media",
            MediaPurpose.Game, MediaType.Image, true)
        {
            References = "References",
            Author = "Author"
        };
        var createdResponse = await SendAsync(createCommand);
        var mediaId = createdResponse.Id;

        var deleteCommand = new DeleteMediaCommand(mediaId);

        // Mock the behavior of DeleteObjectAsync to return false
        _mockObjectStorage.Setup(s => s.DeleteObjectAsync("ObjectName", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () => await SendAsync(deleteCommand));
        Assert.That(exception.Message, Is.EqualTo("Failed to delete media object from bucket"));
    }
}