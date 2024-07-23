using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Events;

namespace Educar.Backend.Application.Commands.Account.CreateAccount;

public record CreateAccountCommand(string Name, string Email, string RegistrationNumber, Guid ClientId, UserRole Role)
    : IRequest<CreatedResponseDto>
{
    public string Name { get; set; } = Name;
    public string Email { get; set; } = Email;
    public string RegistrationNumber { get; set; } = RegistrationNumber;
    public decimal AverageScore { get; set; }
    public decimal EventAverageScore { get; set; }
    public int Stars { get; set; }
    public Guid ClientId { get; set; } = ClientId;
    public UserRole Role { get; set; } = Role;
    public Guid? SchoolId { get; set; }
    public List<Guid>? ClassIds { get; set; }
}

public class CreateAccountCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateAccountCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var client = await context.Clients.FindAsync(new object[] { request.ClientId }, cancellationToken: cancellationToken);
        if (client == null) throw new NotFoundException(nameof(Client), request.ClientId.ToString());

        Domain.Entities.School? school = null;
        if (request.SchoolId != null && request.SchoolId != Guid.Empty)
        {
            school = await context.Schools.FindAsync(new object[] { request.SchoolId }, cancellationToken: cancellationToken);
            if (school == null) throw new NotFoundException(nameof(School), request.SchoolId.ToString()!);
        }

        var classEntities = new List<Domain.Entities.Class>();
        if (request.ClassIds != null && request.ClassIds.Count != 0)
        {
            classEntities = await context.Classes
                .Where(c => request.ClassIds.Contains(c.Id))
                .ToListAsync(cancellationToken);
            
            var missingClassIds = request.ClassIds.Except(classEntities.Select(c => c.Id)).ToList();
            if (missingClassIds.Count != 0)
            {
                throw new NotFoundException(nameof(Class), string.Join(", ", missingClassIds));
            }
        }

        var entity = new Domain.Entities.Account(request.Name, request.Email, request.RegistrationNumber, request.Role)
        {
            AverageScore = request.AverageScore,
            EventAverageScore = request.EventAverageScore,
            Stars = request.Stars,
            Client = client,
            School = school,
        };

        foreach (var classEntity in classEntities)
        {
            entity.AccountClasses.Add(new AccountClass { Account = entity, Class = classEntity });
        }

        entity.AddDomainEvent(new AccountCreatedEvent(entity));
        context.Accounts.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}
