using System.Security.Claims;
using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Web.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    public IList<string>? Roles => _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)
    .Select(c => c.Value)
    .ToList();
}