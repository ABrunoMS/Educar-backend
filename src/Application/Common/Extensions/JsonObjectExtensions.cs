using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Educar.Backend.Application.Commands.AnswerTypes;
using Educar.Backend.Application.Common.NamingPolicies;

namespace Educar.Backend.Application.Common.Extensions;

public static class JsonObjectExtensions
{
    // Default JsonSerializerOptions used across the extension methods
    private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(),
        Converters = 
        {
            new JsonStringEnumConverter(),
            new ExpectedAnswerJsonConverter()
        }
    };

    public static string JsonObjectToString(JsonObject? jsonObject)
    {
        return jsonObject == null ? string.Empty : JsonSerializer.Serialize(jsonObject, DefaultJsonSerializerOptions);
    }

    public static JsonObject StringToJsonObject(string jsonString)
    {
        var jsonNode = JsonSerializer.Deserialize<JsonNode>(jsonString, DefaultJsonSerializerOptions);
        return jsonNode as JsonObject ?? new JsonObject();
    }

    public static JsonObject ToJsonObject<T>(this T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // Use default options
        var jsonString = JsonSerializer.Serialize(entity, DefaultJsonSerializerOptions);
        return JsonNode.Parse(jsonString) as JsonObject ?? new JsonObject();
    }
    
    public static T? ToEntity<T>(this JsonObject jsonObject)
    {
        var jsonString = jsonObject.ToJsonString();
        return JsonSerializer.Deserialize<T>(jsonString, DefaultJsonSerializerOptions);
    }
}
