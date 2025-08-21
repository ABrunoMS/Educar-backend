using Educar.Backend.Domain.Common;
using Educar.Backend.Infrastructure.Data;
namespace Educar.Backend.Domain.Entities;

public class AccountSchool : ISoftDelete 
{
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public Guid SchoolId { get; set; }
    public School School { get; set; } = null!;
    
    // Propriedades de Soft Delete (opcional)
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}