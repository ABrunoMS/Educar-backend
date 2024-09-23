using Microsoft.Extensions.Configuration;

namespace Educar.Backend.Infrastructure.Options;

public class AzureCloudbOptions
{
    public const string SectionName = "AzureCloud";
    public BlobStorageOptions BlobStorage { get; set; } = new();
}

public class BlobStorageOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}

public static class AzureCloudbOptionsExtensions
{
    public static AzureCloudbOptions GetAzureCloudbOptions(this IConfiguration configuration)
    {
        var opt = configuration.GetSection(AzureCloudbOptions.SectionName).Get<AzureCloudbOptions>();
        Guard.Against.Null(opt, message: "AzureCloud section not found.");

        return opt;
    }
}