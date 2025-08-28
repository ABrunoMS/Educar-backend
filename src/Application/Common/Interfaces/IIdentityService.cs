using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Guid> CreateUser(string email, string name, string lastName, string password, UserRole role, CancellationToken cancellationToken);
    Task<bool> DeleteUser(Guid id, CancellationToken cancellationToken);
    Task<bool> TriggerPasswordReset(string username, CancellationToken cancellationToken);
}