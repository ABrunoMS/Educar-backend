namespace Educar.Backend.Application.Common.Interfaces;

public interface IUser
{
    Guid? Id { get; }
    IList<string>? Roles { get; }
}