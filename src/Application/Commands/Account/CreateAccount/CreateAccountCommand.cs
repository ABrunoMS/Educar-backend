using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Events;
using System.Text.Json.Serialization;
using FluentValidation.Results;
using ValidationException = Educar.Backend.Application.Common.Exceptions.ValidationException;

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
    public string? RegistrationNumber { get; init; }

    [JsonPropertyName("averageScore")]
    public decimal AverageScore { get; init; }

    [JsonPropertyName("eventAverageScore")]
    public decimal EventAverageScore { get; init; }

    [JsonPropertyName("stars")]
    public int Stars { get; init; }

    [JsonPropertyName("clientId")]
    public Guid? ClientId { get; init; }

    [JsonPropertyName("role")]
    public UserRole? Role { get; init; }

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
        if (!request.Role.HasValue)
        {
            throw new ValidationException(new List<ValidationFailure> { new ValidationFailure("role", "O campo Role é obrigatório.") });
        }
        
        Domain.Entities.Client? client = null;

        if (request.ClientId.HasValue)
        {
            client = await context.Clients.FindAsync(new object[] { request.ClientId.Value }, cancellationToken: cancellationToken);
            if (client == null) throw new NotFoundException(nameof(Client), request.ClientId.Value.ToString());
        }

        // Validar relacionamentos entre Client, Schools e Classes
        await ValidateClientAndRelationships(request, context, cancellationToken);

        var entity = new Domain.Entities.Account(request.Name, request.Email, request.Role.Value)
        {
            AverageScore = request.AverageScore,
            EventAverageScore = request.EventAverageScore,
            Stars = request.Stars,
            
            // --- MUDANÇA: Passa null se não houver cliente ---
            Client = client, 
            ClientId = request.ClientId, 
            
            Password = request.Password,
            LastName = request.LastName,
            RegistrationNumber = request.RegistrationNumber
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

    private async Task ValidateClientAndRelationships(
        CreateAccountCommand request,
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var failures = new List<ValidationFailure>();

        // Validar se as Schools pertencem ao Client
        if (request.SchoolIds != null && request.SchoolIds.Any() && request.ClientId.HasValue)
        {
            var schools = await context.Schools
                .Where(s => request.SchoolIds.Contains(s.Id))
                .ToListAsync(cancellationToken);

            // Verificar se todas as schools foram encontradas
            if (schools.Count != request.SchoolIds.Count)
            {
                var foundSchoolIds = schools.Select(s => s.Id).ToList();
                var missingSchoolIds = request.SchoolIds.Except(foundSchoolIds).ToList();
                failures.Add(new ValidationFailure("SchoolIds",
                    $"As seguintes escolas não foram encontradas: {string.Join(", ", missingSchoolIds)}"));
            }

            var invalidSchools = schools.Where(s => s.ClientId != request.ClientId.Value).ToList();
            if (invalidSchools.Any())
            {
                var invalidSchoolIds = string.Join(", ", invalidSchools.Select(s => s.Id));
                failures.Add(new ValidationFailure("SchoolIds",
                    $"As seguintes escolas não pertencem ao Client especificado: {invalidSchoolIds}"));
            }
        }

        // Validar se as Classes pertencem às Schools informadas
        if (request.ClassIds != null && request.ClassIds.Any())
        {
            if (request.SchoolIds == null || !request.SchoolIds.Any())
            {
                failures.Add(new ValidationFailure("ClassIds",
                    "Para associar turmas (ClassIds), é necessário informar as escolas (SchoolIds)."));
            }
            else
            {
                var classes = await context.Classes
                    .Include(c => c.School)
                    .Where(c => request.ClassIds.Contains(c.Id))
                    .ToListAsync(cancellationToken);

                // Verificar se todas as classes foram encontradas
                if (classes.Count != request.ClassIds.Count)
                {
                    var foundClassIds = classes.Select(c => c.Id).ToList();
                    var missingClassIds = request.ClassIds.Except(foundClassIds).ToList();
                    failures.Add(new ValidationFailure("ClassIds",
                        $"As seguintes turmas não foram encontradas: {string.Join(", ", missingClassIds)}"));
                }

                var invalidClasses = classes.Where(c => !request.SchoolIds.Contains(c.SchoolId)).ToList();
                if (invalidClasses.Any())
                {
                    var invalidClassDetails = string.Join(", ",
                        invalidClasses.Select(c => $"{c.Id} (School: {c.School.Name})"));
                    failures.Add(new ValidationFailure("ClassIds",
                        $"As seguintes turmas não pertencem às escolas especificadas: {invalidClassDetails}"));
                }
            }
        }

        if (failures.Any())
        {
            throw new ValidationException(failures);
        }
    }
}
