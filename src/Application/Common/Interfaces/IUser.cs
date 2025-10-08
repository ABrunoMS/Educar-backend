namespace Educar.Backend.Application.Common.Interfaces;

public interface IUser
{
    string? Id { get; }
    IList<string>? Roles { get; }
}