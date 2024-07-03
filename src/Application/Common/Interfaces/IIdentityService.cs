namespace Educar.Backend.Application.Interfaces;

public interface IIdentityService
{
    public Task<bool> CreateRole(string roleName, CancellationToken cancellationToken);
}