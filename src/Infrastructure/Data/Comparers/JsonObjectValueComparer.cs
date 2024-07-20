namespace Educar.Backend.Infrastructure.Data.Comparers;

using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public static class JsonObjectValueComparer
{
    public static readonly ValueComparer<JsonObject> Instance = new(
        (c1, c2) => JsonObjectComparer(c1, c2),
        c => c != null ? c.ToJsonString(null).GetHashCode() : 0,
        c => c != null
            ? JsonNode.Parse(c.ToJsonString(default), default, default) as JsonObject ?? new JsonObject(default)
            : new JsonObject(default));

    private static bool JsonObjectComparer(JsonObject? obj1, JsonObject? obj2)
    {
        if (ReferenceEquals(obj1, obj2))
        {
            return true;
        }

        if (obj1 == null || obj2 == null)
        {
            return false;
        }

        return obj1.ToJsonString() == obj2.ToJsonString();
    }
}