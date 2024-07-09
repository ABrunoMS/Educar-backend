using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Common.Interfaces;

public interface IIdentityService
{
    public Task<Guid> CreateUser(string email, string name, UserRole role, CancellationToken cancellationToken);
}