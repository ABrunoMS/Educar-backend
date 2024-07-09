using System.Text.Json;
using Educar.Backend.Application.Extensions;

namespace Educar.Backend.Web.Infrastructure;

public class JsonSnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name.ToSnakeCase();
    }
}