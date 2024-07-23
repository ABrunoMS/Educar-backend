using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using ValidationException = Educar.Backend.Application.Common.Exceptions.ValidationException;

namespace Educar.Backend.Application.Commands.Account.UpdateAccount;

public record UpdateAccountCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? RegistrationNumber { get; set; }
    public decimal AverageScore { get; set; }
    public decimal EventAverageScore { get; set; }
    public int Stars { get; set; }
    public Guid? SchoolId { get; set; }
    public IList<Guid> ClassIds { get; set; } = new List<Guid>();
}

public class UpdateAccountCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateAccountCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Accounts
            .Include(a => a.AccountClasses)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        var school = await GetEntityByIdAsync(context.Schools, request.SchoolId, cancellationToken);

        var classEntities = await context.Classes
            .Where(c => request.ClassIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        ValidateNonAdminRole(entity.Role, request.SchoolId, request.ClassIds);

        UpdateEntity(entity, request, school, classEntities, context);

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private async Task<T?> GetEntityByIdAsync<T>(DbSet<T> dbSet, Guid? id, CancellationToken cancellationToken)
        where T : class
    {
        if (id == null || id == Guid.Empty) return null;

        var entity = await dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null) throw new NotFoundException(typeof(T).Name, id.ToString()!);
        return entity;
    }

    private void ValidateNonAdminRole(UserRole role, Guid? schoolId, IList<Guid> classIds)
    {
        if (role == UserRole.Admin) return;

        var exception = new ValidationException();
        if (schoolId == null || schoolId == Guid.Empty)
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

    private void UpdateEntity(Domain.Entities.Account entity, UpdateAccountCommand request,
        Domain.Entities.School? school, List<Domain.Entities.Class> classEntities, IApplicationDbContext context)
    {
        if (request.Name != null) entity.Name = request.Name;
        if (request.RegistrationNumber != null) entity.RegistrationNumber = request.RegistrationNumber;
        entity.AverageScore = request.AverageScore;
        entity.EventAverageScore = request.EventAverageScore;
        entity.Stars = request.Stars;
        entity.School = school;

        // Use IgnoreQueryFilters to get all AccountClasses including soft-deleted ones
        var allAccountClasses = context.AccountClasses
            .IgnoreQueryFilters()
            .Where(ac => ac.AccountId == entity.Id)
            .ToList();

        var currentClassIds = allAccountClasses.Where(ac => !ac.IsDeleted).Select(ac => ac.ClassId).ToList();
        var newClassIds = request.ClassIds;

        // Find classes to add
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
                entity.AccountClasses.Add(new AccountClass { AccountId = entity.Id, ClassId = classId });
            }
        }

        // Find classes to remove (soft delete)
        var classesToRemove = currentClassIds.Except(newClassIds).ToList();
        foreach (var accountClass in classesToRemove.Select(classId =>
                     entity.AccountClasses.First(ac => ac.ClassId == classId)))
        {
            accountClass.IsDeleted = true;
            accountClass.DeletedAt = DateTimeOffset.UtcNow;
        }
    }
}