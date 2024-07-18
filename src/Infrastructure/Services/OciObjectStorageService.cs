using System.Text;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oci.Common;
using Oci.Common.Auth;
using Oci.ObjectstorageService;
using Oci.ObjectstorageService.Requests;

namespace Educar.Backend.Infrastructure.Services;

public class OciObjectStorageService : IObjectStorage
{
    private readonly ILogger<OciObjectStorageService> _logger;
    private readonly ObjectStorageClient _objectStorageClient;
    private readonly OracleCloudOptions _oracleCloudOptions;
    private string _bucketUrlPrefix;

    public OciObjectStorageService(IConfiguration configuration, ILogger<OciObjectStorageService> logger)
    {
        _logger = logger;
        _oracleCloudOptions = configuration.GetOracleCloudOptions();
        _bucketUrlPrefix = _oracleCloudOptions.BucketUrl
            .Replace("{Namespace}", _oracleCloudOptions.BucketNamespace)
            .Replace("{BucketName}", _oracleCloudOptions.BucketName);

        var keyBytes = Convert.FromBase64String(_oracleCloudOptions.KeyBase64);
        var privateKey = Encoding.UTF8.GetString(keyBytes);

        var provider = new SimpleAuthenticationDetailsProvider
        {
            TenantId = _oracleCloudOptions.TenancyOcid,
            UserId = _oracleCloudOptions.UserOcid,
            Fingerprint = _oracleCloudOptions.Fingerprint,
            Region = Region.FromRegionId(_oracleCloudOptions.Region),
            PrivateKeySupplier = new PrivateKeySupplier(privateKey)
        };

        _objectStorageClient = new ObjectStorageClient(provider);
    }

    public async Task<string?> PutObjectAsync(string objectName, Stream stream, CancellationToken cancellationToken)
    {
        string? url = null;
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = _oracleCloudOptions.BucketName,
            NamespaceName = _oracleCloudOptions.BucketNamespace,
            ObjectName = objectName,
            PutObjectBody = stream
        };

        try
        {
            await _objectStorageClient.PutObject(putObjectRequest, cancellationToken: cancellationToken);
            url = _bucketUrlPrefix + objectName;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed at PutObjectAsync: {e}", e);
        }

        return url;
    }
}