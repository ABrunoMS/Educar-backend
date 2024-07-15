using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Domain.Entities;

public class Account(string name, string email, string registrationNumber, UserRole role) : BaseAuditableEntity
{
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string RegistrationNumber { get; set; } = registrationNumber;
    public decimal AverageScore { get; set; }
    public decimal EventAverageScore { get; set; }
    public int Stars { get; set; }
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public Guid? SchoolId { get; set; }
    public School? School { get; set; }

    public UserRole Role { get; set; } = role;
}