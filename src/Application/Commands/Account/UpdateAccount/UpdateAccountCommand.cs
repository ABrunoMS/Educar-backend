using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using ValidationException = Educar.Backend.Application.Common.Exceptions.ValidationException;

namespace Educar.Backend.Application.Commands.Account.UpdateAccount;

public record UpdateAccountCommand : IRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? RegistrationNumber { get; set; }
    public decimal AverageScore { get; set; }
    public decimal EventAverageScore { get; set; }
    public int Stars { get; set; }
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

        ValidateNonAdminRole(entity.Role, request.SchoolIds, request.ClassIds);

        UpdateEntity(entity, request, context);

        await context.SaveChangesAsync(cancellationToken);
    }

    private void ValidateNonAdminRole(UserRole role, IList<Guid> schoolIds, IList<Guid> classIds)
    {
        if (role == UserRole.Admin) return;
        var exception = new ValidationException();
        if (schoolIds == null || schoolIds.Count == 0)
        {
          exception.Errors.Add("SchoolId", ["School ID is required for non-admin roles."]);
        }
        if (classIds == null || classIds.Count == 0)
        {
          exception.Errors.Add("ClassIds", ["At least one Class ID is required for non-admin roles."]);
        }  
        if (exception.Errors.Any())
        {
        throw exception;
        }
    }
    
    private void UpdateEntity(Domain.Entities.Account entity, UpdateAccountCommand request, IApplicationDbContext context)
    {
        // 1. Atualiza os campos simples
        if (request.Name != null) entity.Name = request.Name;
        if (request.RegistrationNumber != null) entity.RegistrationNumber = request.RegistrationNumber;
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