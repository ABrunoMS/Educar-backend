using System.Text.Json;
using System.Text.Json.Nodes;

namespace Educar.Backend.Application.Common.Extensions;

public static class JsonObjectExtensions
{
    public static string JsonObjectToString(JsonObject? jsonObject)
    {
        return jsonObject == null ? string.Empty : jsonObject.ToJsonString();
    }

    public static JsonObject StringToJsonObject(string jsonString)
    {
        return JsonNode.Parse(jsonString) as JsonObject ?? new JsonObject();
    }

    public static JsonObject ToJsonObject<T>(this T entity)
    {
        var jsonString = JsonSerializer.Serialize(entity);
        return JsonNode.Parse(jsonString) as JsonObject ?? new JsonObject();
    }
    
    public static T? ToEntity<T>(this JsonObject jsonObject)
    {
        var jsonString = jsonObject.ToJsonString();
        return JsonSerializer.Deserialize<T>(jsonString);
    }
}