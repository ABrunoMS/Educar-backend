using Microsoft.Extensions.Configuration;

namespace Educar.Backend.Infrastructure.Options;

public class OracleCloudOptions
{
    public const string SectionName = "OracleCloud";

    public OracleCloudOptions(string userOcid, string fingerprint, string keyBase64, string tenancyOcid, string region,
        string bucketName, string bucketNamespace, string bucketUrl)
    {
        UserOcid = userOcid;
        Fingerprint = fingerprint;
        KeyBase64 = keyBase64;
        TenancyOcid = tenancyOcid;
        Region = region;
        BucketName = bucketName;
        BucketNamespace = bucketNamespace;
        BucketUrl = bucketUrl;
    }

    public string UserOcid { get; set; }
    public string Fingerprint { get; set; }
    public string KeyBase64 { get; set; }
    public string TenancyOcid { get; set; }
    public string Region { get; set; }
    public string BucketName { get; set; }
    public string BucketNamespace { get; set; }
    public string BucketUrl { get; set; }
}

public static class OracleCloudOptionsExtensions
{
    public static OracleCloudOptions GetOracleCloudOptions(this IConfiguration configuration)
    {
        var opt = configuration.GetSection(OracleCloudOptions.SectionName).Get<OracleCloudOptions>();
        Guard.Against.Null(opt, message: "OracleCloud section not found.");

        return opt;
    }
}