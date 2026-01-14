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

    public Guid? Id
    {
        get
        {
            var idClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(idClaim, out var guid) ? guid : null;
        }
    }
    
    public IList<string>? Roles => _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)
    .Select(c => c.Value)
    .ToList();
}