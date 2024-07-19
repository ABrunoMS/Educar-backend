using Ardalis.GuardClauses;
using Educar.Backend.Application.Commands.Media.CreateMedia;
using Educar.Backend.Application.Commands.Media.UpdateMedia;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Queries.Media;
using Educar.Backend.Domain.Enums;
using Moq;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Media;

[TestFixture]
public class UpdateMediaTests : TestBase
{
    private Mock<IObjectStorage> _mockObjectStorage;

    [SetUp]
    public void SetUp()
    {
        ResetState();
        _mockObjectStorage = MockObjectStorage;
    }

    [Test]
    public async Task GivenValidRequest_ShouldUpdateMedia()
    {
        // Arrange
        var createCommand = new CreateMediaCommand("Initial Media", "InitialObject", "https://example.com/initial",
            MediaPurpose.Game, MediaType.Image, true)
        {
            References = "Initial References",
            Author = "Initial Author"
        };
        var createdResponse = await SendAsync(createCommand);
        var mediaId = createdResponse.Id;

        var updateCommand = new UpdateMediaCommand
        {
            Id = mediaId,
            Name = "Updated Media",
            ObjectName = "UpdatedObject",
            Url = "https://example.com/updated",
            Purpose = MediaPurpose.Game,
            Type = MediaType.Video,
            References = "Updated References",
            Author = "Updated Author",
            Agreement = true
        };

        // Mock the behavior of CheckObjectExistsAsync and DeleteObjectAsync
        _mockObjectStorage.Setup(s => s.CheckObjectExistsAsync("UpdatedObject", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockObjectStorage.Setup(s => s.DeleteObjectAsync("InitialObject", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await SendAsync(updateCommand);
        var query = new GetMediaQuery { Id = mediaId };
        var updatedMedia = await SendAsync(query);

        // Assert
        Assert.That(updatedMedia, Is.Not.Null);
        Assert.That(updatedMedia.Name, Is.EqualTo("Updated Media"));
        Assert.That(updatedMedia.ObjectName, Is.EqualTo("UpdatedObject"));
        Assert.That(updatedMedia.Url, Is.EqualTo("https://example.com/updated"));
        Assert.That(updatedMedia.Purpose, Is.EqualTo(MediaPurpose.Game));
        Assert.That(updatedMedia.Type, Is.EqualTo(MediaType.Video));
        Assert.That(updatedMedia.References, Is.EqualTo("Updated References"));
        Assert.That(updatedMedia.Author, Is.EqualTo("Updated Author"));
        Assert.That(updatedMedia.Agreement, Is.True);
    }

    [Test]
    public void GivenInvalidId_ShouldThrowNotFoundException()
    {
        
        var updateCommand = new UpdateMediaCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Media",
            ObjectName = "UpdatedObject",
            Url = "https://example.com/updated",
            Purpose = MediaPurpose.Game,
            Type = MediaType.Video,
            Agreement = true
        };
        
        _mockObjectStorage.Setup(s => s.CheckObjectExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Assert.ThrowsAsync<NotFoundException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void GivenEmptyName_ShouldThrowValidationException()
    {
        var updateCommand = new UpdateMediaCommand
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            ObjectName = "UpdatedObject",
            Url = "https://example.com/updated",
            Purpose = MediaPurpose.Game,
            Type = MediaType.Video,
            Agreement = true
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void GivenInvalidUrl_ShouldThrowValidationException()
    {
        var updateCommand = new UpdateMediaCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Media",
            ObjectName = "UpdatedObject",
            Url = "invalid-url",
            Purpose = MediaPurpose.Game,
            Type = MediaType.Video,
            Agreement = true
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public async Task GivenNonExistentObjectName_ShouldThrowValidationException()
    {
        // Arrange
        var createCommand = new CreateMediaCommand("Initial Media", "InitialObject", "https://example.com/initial",
            MediaPurpose.Game, MediaType.Image, true)
        {
            References = "Initial References",
            Author = "Initial Author"
        };
        var createdResponse = await SendAsync(createCommand);
        var mediaId = createdResponse.Id;

        var updateCommand = new UpdateMediaCommand
        {
            Id = mediaId,
            ObjectName = "NonExistentObject"
        };

        // Mock the behavior of CheckObjectExistsAsync to return false
        _mockObjectStorage.Setup(s => s.CheckObjectExistsAsync("NonExistentObject", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void GivenAuthorExceedsMaxLength_ShouldThrowValidationException()
    {
        var longAuthor = new string('a', 101);
        var updateCommand = new UpdateMediaCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Media",
            ObjectName = "UpdatedObject",
            Url = "https://example.com/updated",
            Purpose = MediaPurpose.Game,
            Type = MediaType.Video,
            Agreement = true,
            Author = longAuthor
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }

    [Test]
    public void GivenAgreementFalse_ShouldThrowValidationException()
    {
        var updateCommand = new UpdateMediaCommand
        {
            Id = Guid.NewGuid(),
            Name = "Updated Media",
            ObjectName = "UpdatedObject",
            Url = "https://example.com/updated",
            Purpose = MediaPurpose.Game,
            Type = MediaType.Video,
            Agreement = false
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(updateCommand));
    }
}