using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using FluentValidation.Results;
using ValidationException = Educar.Backend.Application.Common.Exceptions.ValidationException;

namespace Educar.Backend.Application.Commands.Account.UpdateAccount;

public record UpdateAccountCommand : IRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? RegistrationNumber { get; set; }
    public decimal AverageScore { get; set; }
    public decimal EventAverageScore { get; set; }
    public int Stars { get; set; }
    public UserRole? Role { get; set; }
    public Guid? ClientId { get; set; }
    public IList<Guid> SchoolIds { get; set; } = new List<Guid>();
    public IList<Guid> ClassIds { get; set; } = new List<Guid>();
}

public class UpdateAccountCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateAccountCommand>
{
    public async Task Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Accounts
            .Include(a => a.AccountClasses)
            .Include(a => a.AccountSchools)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        
        Guard.Against.NotFound(request.Id, entity);

        // Validar ClientId e relacionamentos
        await ValidateClientAndRelationships(request, entity, context, cancellationToken);

        UpdateEntity(entity, request, context);

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateClientAndRelationships(
        UpdateAccountCommand request, 
        Domain.Entities.Account entity,
        IApplicationDbContext context, 
        CancellationToken cancellationToken)
    {
        var failures = new List<ValidationFailure>();
        
        // Determinar o ClientId a ser usado (novo ou atual)
        var clientIdToValidate = request.ClientId ?? entity.ClientId;
        
        // Se está tentando atualizar o ClientId, validar se ele existe
        if (request.ClientId.HasValue && request.ClientId.Value != entity.ClientId)
        {
            var clientExists = await context.Clients.AnyAsync(c => c.Id == request.ClientId.Value, cancellationToken);
            if (!clientExists)
            {
                failures.Add(new ValidationFailure("ClientId", "O Client informado não existe."));
            }
        }
        
        // Validar se as Schools pertencem ao Client
        if (request.SchoolIds != null && request.SchoolIds.Any() && clientIdToValidate.HasValue)
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
            
            var invalidSchools = schools.Where(s => s.ClientId != clientIdToValidate.Value).ToList();
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
            // Se não há SchoolIds no request, usar as schools atuais da conta
            var schoolIdsToValidate = (request.SchoolIds != null && request.SchoolIds.Any()) 
                ? request.SchoolIds 
                : entity.AccountSchools.Where(asc => !asc.IsDeleted).Select(asc => asc.SchoolId).ToList();
            
            // Verificar se há escolas para validar
            if (!schoolIdsToValidate.Any())
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
                
                var invalidClasses = classes.Where(c => !schoolIdsToValidate.Contains(c.SchoolId)).ToList();
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
    
    private void UpdateEntity(Domain.Entities.Account entity, UpdateAccountCommand request, IApplicationDbContext context)
    {
        // 1. Atualiza os campos simples
        if (request.Name != null) entity.Name = request.Name;
        if (request.LastName != null) entity.LastName = request.LastName;
        if (request.RegistrationNumber != null) entity.RegistrationNumber = request.RegistrationNumber;
        if (request.Role.HasValue) entity.Role = request.Role.Value;
        if (request.ClientId.HasValue) entity.ClientId = request.ClientId.Value;
        entity.AverageScore = request.AverageScore;
        entity.EventAverageScore = request.EventAverageScore;
        entity.Stars = request.Stars;

        // 2. LÓGICA DE SOFT DELETE PARA ESCOLAS 
        var allAccountSchools = context.AccountSchools
            .IgnoreQueryFilters()
            .Where(asc => asc.AccountId == entity.Id)
            .ToList();
        
        var currentSchoolIds = allAccountSchools.Where(asc => !asc.IsDeleted).Select(asc => asc.SchoolId).ToList();
        var newSchoolIds = request.SchoolIds;

        var schoolsToAdd = newSchoolIds.Except(currentSchoolIds).ToList();
        foreach (var schoolId in schoolsToAdd)
        {
            var existingAccountSchool = allAccountSchools.FirstOrDefault(asc => asc.SchoolId == schoolId && asc.IsDeleted);
            if (existingAccountSchool != null)
            {
                existingAccountSchool.IsDeleted = false;
                existingAccountSchool.DeletedAt = null;
            }
            else
            {
                entity.AccountSchools.Add(new AccountSchool { SchoolId = schoolId });
            }
        }

        var schoolsToRemove = currentSchoolIds.Except(newSchoolIds).ToList();
        foreach (var accountSchool in allAccountSchools.Where(asc => schoolsToRemove.Contains(asc.SchoolId)))
        {
            accountSchool.IsDeleted = true;
            accountSchool.DeletedAt = DateTimeOffset.UtcNow;
        }

        // 3. LÓGICA DE SOFT DELETE PARA TURMAS 
        var allAccountClasses = context.AccountClasses
            .IgnoreQueryFilters()
            .Where(ac => ac.AccountId == entity.Id)
            .ToList();

        var currentClassIds = allAccountClasses.Where(ac => !ac.IsDeleted).Select(ac => ac.ClassId).ToList();
        var newClassIds = request.ClassIds;

        var classesToAdd = newClassIds.Except(currentClassIds).ToList();
        foreach (var classId in classesToAdd)
        {
            var existingAccountClass = allAccountClasses.FirstOrDefault(ac => ac.ClassId == classId && ac.IsDeleted);
            if (existingAccountClass != null)
            {
                existingAccountClass.IsDeleted = false;
                existingAccountClass.DeletedAt = null;
            }
            else
            {
                entity.AccountClasses.Add(new AccountClass { ClassId = classId });
            }
        }

        var classesToRemove = currentClassIds.Except(newClassIds).ToList();
        foreach (var accountClass in allAccountClasses.Where(ac => classesToRemove.Contains(ac.ClassId)))
        {
            accountClass.IsDeleted = true;
            accountClass.DeletedAt = DateTimeOffset.UtcNow;
        }
    }
}
