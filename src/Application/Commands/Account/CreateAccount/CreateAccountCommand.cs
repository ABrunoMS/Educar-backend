using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Events;
using System.Text.Json.Serialization;

namespace Educar.Backend.Application.Commands.Account.CreateAccount;

public record CreateAccountCommand : IRequest<IdResponseDto>
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string? LastName { get; init; }

    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;
    
    [JsonPropertyName("password")] 
    public string Password { get; init; } = string.Empty;

    [JsonPropertyName("registrationNumber")]
    public string RegistrationNumber { get; init; } = string.Empty;

    [JsonPropertyName("averageScore")]
    public decimal AverageScore { get; init; }

    [JsonPropertyName("eventAverageScore")]
    public decimal EventAverageScore { get; init; }

    [JsonPropertyName("stars")]
    public int Stars { get; init; }

    [JsonPropertyName("clientId")]
    public Guid ClientId { get; init; }

    [JsonPropertyName("role")]
    public UserRole Role { get; init; }

    [JsonPropertyName("schoolIds")]
    public List<Guid>? SchoolIds { get; init; }

    [JsonPropertyName("classIds")]
    public List<Guid>? ClassIds { get; init; }
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
            ClientId = request.ClientId,
            Password = request.Password,
            LastName = request.LastName,
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

