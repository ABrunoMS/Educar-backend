using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Events;

namespace Educar.Backend.Application.Commands.Account.CreateAccount;

public record CreateAccountCommand(string Name, string Email, string RegistrationNumber, Guid ClientId, UserRole Role)
    : IRequest<IdResponseDto>
{
    public string Name { get; set; } = Name;
    public string Email { get; set; } = Email;
    public string RegistrationNumber { get; set; } = RegistrationNumber;
    public decimal AverageScore { get; set; }
    public decimal EventAverageScore { get; set; }
    public int Stars { get; set; }
    public Guid ClientId { get; set; } = ClientId;
    public UserRole Role { get; set; } = Role;
    //public Guid? SchoolId { get; set; }
    public List<Guid>? SchoolIds { get; init; }
    public List<Guid>? ClassIds { get; set; }
}

public class CreateAccountCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateAccountCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        
        var client = await context.Clients.FindAsync(new object[] { request.ClientId }, cancellationToken: cancellationToken);
        if (client == null) throw new NotFoundException(nameof(Client), request.ClientId.ToString());
        

        var entity = new Domain.Entities.Account(request.Name, request.Email, request.RegistrationNumber, request.Role)
        {
            AverageScore = request.AverageScore,
            EventAverageScore = request.EventAverageScore,
            Stars = request.Stars,
            Client = client,
            ClientId = request.ClientId
        };

        
        if (request.SchoolIds != null && request.SchoolIds.Any())
        {
            foreach (var schoolId in request.SchoolIds)
            {
                entity.AccountSchools.Add(new AccountSchool { SchoolId = schoolId });
            }
        }

        
        if (request.ClassIds != null && request.ClassIds.Any())
        {
            foreach (var classId in request.ClassIds)
            {
                entity.AccountClasses.Add(new AccountClass { ClassId = classId });
            }
        } 

        
        entity.AddDomainEvent(new AccountCreatedEvent(entity));
        context.Accounts.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}

