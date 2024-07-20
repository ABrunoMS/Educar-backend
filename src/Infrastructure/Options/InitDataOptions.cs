using Microsoft.Extensions.Configuration;

namespace Educar.Backend.Infrastructure.Options;

public class InitDataOptions(string defaultAdminPassword)
{
    public const string SectionName = "InitData";

    public bool Active { get; set; }
    public string DefaultAdminPassword { get; set; } = defaultAdminPassword;
}

public static class InitDataOptionsExtensions
{
    public static InitDataOptions GetInitDataOptions(this IConfiguration configuration)
    {
        var opt = configuration.GetSection(InitDataOptions.SectionName).Get<InitDataOptions>();
        Guard.Against.Null(opt, message: "InitData section not found.");

        return opt;
    }
}