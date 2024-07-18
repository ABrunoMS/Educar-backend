using System.Text;
using Educar.Backend.Application.Commands.Media.UploadFileCommand;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Application.Common.Interfaces;
using Moq;
using NUnit.Framework;
using static Educar.Backend.Application.IntegrationTests.Testing;

namespace Educar.Backend.Application.IntegrationTests.Media;

[TestFixture]
public class UploadMediaTests : TestBase
{
    private Mock<IObjectStorage> _mockObjectStorageService;

    [SetUp]
    public void SetUp()
    {
        _mockObjectStorageService = new Mock<IObjectStorage>();
    }

    [Test]
    public async Task GivenValidRequest_ShouldUploadFile()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("File content"));
        var extension = "txt";
        var command = new UploadFileCommand(stream, extension);
        var expectedUrl = "https://objectstorage.example.com/file.txt";

        _mockObjectStorageService
            .Setup(s => s.PutObjectAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUrl);

        var handler = new UploadFileCommandHandler(_mockObjectStorageService.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Url, Is.EqualTo(expectedUrl));
        Assert.That(response.ObjectName, Does.EndWith($".{extension}"));

        _mockObjectStorageService.Verify(
            s => s.PutObjectAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void ShouldThrowException_WhenObjectStorageFails()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("File content"));
        var extension = "txt";
        var command = new UploadFileCommand(stream, extension);

        _mockObjectStorageService
            .Setup(s => s.PutObjectAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string)null!); // Simulate failure by returning a nullable string

        var handler = new UploadFileCommandHandler(_mockObjectStorageService.Object);

        // Act & Assert
        var exception =
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await handler.Handle(command, CancellationToken.None));
        Assert.That(exception!.Message, Does.Contain("Failed to upload file to object storage."));
    }

    [Test]
    public void ShouldThrowValidationException_WhenExtensionIsEmpty()
    {
        // Arrange
        var stream = new MemoryStream("File content"u8.ToArray());
        var extension = string.Empty;
        var command = new UploadFileCommand(stream, extension);

        Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}