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

        // return roleNames.Any(roleName => lowerCaseRoles.Contains(roleName.ToString().ToLower())); ISSO ERA ANTES
        
        if (lowerCaseRoles.Contains(UserRole.Admin.ToString().ToLower()))
            return true;


        if (roleNames.Any(roleName => lowerCaseRoles.Contains(roleName.ToString().ToLower())))
            return true;


        foreach (var roleName in roleNames)
        {
            switch (roleName)
            {
                case UserRole.Teacher:
                    if (lowerCaseRoles.Contains(UserRole.TeacherEducar.ToString().ToLower()) ||
                        lowerCaseRoles.Contains(UserRole.Distribuidor.ToString().ToLower()) ||
                        lowerCaseRoles.Contains(UserRole.AgenteComercial.ToString().ToLower()))
                        return true;
                    break;
                    
                case UserRole.Secretario:
                    if (lowerCaseRoles.Contains(UserRole.AgenteComercial.ToString().ToLower()))
                        return true;
                    break;
            }
        }

        return false;
    }
}