using Microsoft.Extensions.Configuration;

namespace Educar.Backend.Infrastructure.Options;

public class AuthOptions
{
    public const string SectionName = "Auth";

    public AuthOptions(string issuer, bool requireHttpsMetadata, string tokenUrl, string adminClientId,
        string adminClientSecret, string adminUrl)
    {
        Issuer = issuer;
        RequireHttpsMetadata = requireHttpsMetadata;
        TokenUrl = tokenUrl;
        AdminClientId = adminClientId;
        AdminClientSecret = adminClientSecret;
        AdminUrl = adminUrl;
    }

    public string Issuer { get; set; }
    public bool RequireHttpsMetadata { get; set; }
    public string TokenUrl { get; set; }
    public string AdminUrl { get; set; }
    public string AdminClientId { get; set; }
    public string AdminClientSecret { get; set; }
}

public static class AuthOptionsExtensions
{
    public static AuthOptions GetAuthOptions(this IConfiguration configuration)
    {
        var opt = configuration.GetSection(AuthOptions.SectionName).Get<AuthOptions>();
        Guard.Against.Null(opt, message: "Auth section not found.");

        return opt;
    }
}