namespace Educar.Backend.Infrastructure.Extensions;

public static class ObjectExtensions
{
    public static string ToSnakeCase(this string str)
    {
        return string.Concat(
            str.Select((x, i) =>
                i > 0 && char.IsUpper(x) &&
                (char.IsLower(str[i - 1]) || (i < str.Length - 1 && char.IsLower(str[i + 1])))
                    ? "_" + x
                    : x.ToString())).ToLowerInvariant();
    }
}