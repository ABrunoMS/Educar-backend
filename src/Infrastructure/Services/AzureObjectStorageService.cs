using Azure.Storage.Blobs;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Educar.Backend.Infrastructure.Services;

public class AzureBlobStorageService : IObjectStorage
{
    private readonly ILogger<AzureBlobStorageService> _logger;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public AzureBlobStorageService(IConfiguration configuration, ILogger<AzureBlobStorageService> logger)
    {
        _logger = logger;
        var azureBlobOptions = configuration.GetAzureCloudbOptions();
        _blobServiceClient = new BlobServiceClient(azureBlobOptions.BlobStorage.ConnectionString);
        _containerName = azureBlobOptions.BlobStorage.ContainerName;
    }

    public async Task<string?> PutObjectAsync(string objectName, Stream stream, CancellationToken cancellationToken)
    {
        string? url = null;
        try
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(objectName);

            await blobClient.UploadAsync(stream, overwrite: true, cancellationToken: cancellationToken);
            url = blobClient.Uri.ToString();
        }
        catch (Exception e)
        {
            _logger.LogError("Failed at PutObjectAsync: {e}", e);
        }

        return url;
    }

    public async Task<bool> DeleteObjectAsync(string objectName, CancellationToken cancellationToken)
    {
        try
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(objectName);

            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed at DeleteObjectAsync: {e}", e);
            return false;
        }

        return true;
    }

    public async Task<bool> CheckObjectExistsAsync(string objectName, CancellationToken cancellationToken)
    {
        try
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(objectName);

            var properties = await blobClient.ExistsAsync(cancellationToken: cancellationToken);
            return properties.Value;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed at CheckObjectExistsAsync: {e}", e);
            return false;
        }
    }
}