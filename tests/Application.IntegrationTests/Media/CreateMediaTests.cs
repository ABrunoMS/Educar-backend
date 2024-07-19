using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Media.CreateMedia;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Enums;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Media;

[TestFixture]
public class CreateMediaTests : TestBase
{
    [SetUp]
    public void SetUp()
    {
        ResetState();
    }

    [Test]
    public async Task GivenValidRequest_ShouldCreateMedia()
    {
        const string name = "New Media";
        const string objectName = "ObjectName";
        const string url = "https://example.com/media";
        const MediaPurpose purpose = MediaPurpose.Game;
        const MediaType type = MediaType.Image;
        const bool agreement = true;

        const string references = "Some references";
        const string author = "Author name";

        // Arrange
        var command = new CreateMediaCommand(name, objectName, url, purpose, type, agreement)
        {
            References = references,
            Author = author
        };

        // Act
        var response = await SendAsync(command);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<CreatedResponseDto>());

        var createdMedia = await Context.Medias.FindAsync(response.Id);
        Assert.That(createdMedia, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createdMedia.Name, Is.EqualTo(name));
            Assert.That(createdMedia.ObjectName, Is.EqualTo(objectName));
            Assert.That(createdMedia.Url, Is.EqualTo(url));
            Assert.That(createdMedia.Purpose, Is.EqualTo(purpose));
            Assert.That(createdMedia.Type, Is.EqualTo(type));
            Assert.That(createdMedia.Agreement, Is.True);
            Assert.That(createdMedia.References, Is.EqualTo(references));
            Assert.That(createdMedia.Author, Is.EqualTo(author));
        });
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameIsEmpty()
    {
        var command = new CreateMediaCommand(string.Empty, "ObjectName", "https://example.com/media", MediaPurpose.Game,
            MediaType.Image, true);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var longName = new string('a', 101);
        var command = new CreateMediaCommand(longName, "ObjectName", "https://example.com/media", MediaPurpose.Game,
            MediaType.Image, true);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenObjectNameIsEmpty()
    {
        var command = new CreateMediaCommand("Media Name", string.Empty, "https://example.com/media", MediaPurpose.Game,
            MediaType.Image, true);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenUrlIsEmpty()
    {
        var command = new CreateMediaCommand("Media Name", "ObjectName", string.Empty, MediaPurpose.Game,
            MediaType.Image, true);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenUrlIsInvalid()
    {
        var invalidUrl = "invalid-url";
        var command = new CreateMediaCommand("Media Name", "ObjectName", invalidUrl, MediaPurpose.Game, MediaType.Image,
            true);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenPurposeIsNull()
    {
        var command = new CreateMediaCommand("Media Name", "ObjectName", "https://example.com/media",
            default(MediaPurpose), MediaType.Image, true);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenTypeIsEmpty()
    {
        var command = new CreateMediaCommand("Media Name", "ObjectName", "https://example.com/media", MediaPurpose.Game,
            default(MediaType), true);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenAgreementIsFalse()
    {
        var command = new CreateMediaCommand("Media Name", "ObjectName", "https://example.com/media", MediaPurpose.Game,
            MediaType.Image, false);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Test]
    public void ShouldThrowValidationException_WhenAuthorExceedsMaxLength()
    {
        var longAuthor = new string('a', 101);
        var command = new CreateMediaCommand("Media Name", "ObjectName", "https://example.com/media", MediaPurpose.Game,
            MediaType.Image, true)
        {
            Author = longAuthor
        };

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}