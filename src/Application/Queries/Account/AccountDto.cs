using Educar.Backend.Application.Queries.Client;
using Educar.Backend.Application.Queries.School;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.Account;

public class AccountDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? RegistrationNumber { get; set; }
    public decimal AverageScore { get; set; }
    public decimal EventAverageScore { get; set; }
    public int Stars { get; set; }
    public Guid ClientId { get; set; }
    public UserRole Role { get; set; }
    
    public SchoolDto? School { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Account, AccountDto>();
        }
    }
}