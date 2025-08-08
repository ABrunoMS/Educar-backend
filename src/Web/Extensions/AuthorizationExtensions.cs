using System.Security.Claims;
using System.Text.Json;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Web.Extensions;

public static class AuthorizationExtensions
{
    public static bool HasRoles(this ClaimsPrincipal user, List<UserRole> roleNames)
    {
        var realmAccessClaim = user.FindFirst(claim => claim.Type == "realm_access")?.Value;

        if (string.IsNullOrEmpty(realmAccessClaim)) return false;

        var realmAccessAsDict = JsonSerializer.Deserialize<Dictionary<string, string[]>>(realmAccessClaim);
        if (realmAccessAsDict == null || !realmAccessAsDict.TryGetValue("roles", out var roles)) return false;

        var lowerCaseRoles = roles.Select(role => role.ToLower()).ToList();
        return roleNames.Any(roleName => lowerCaseRoles.Contains(roleName.ToString().ToLower()));
    }
}