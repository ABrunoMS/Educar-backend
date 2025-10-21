using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Class.UpdateClass;

public record UpdateClassCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ClassPurpose? Purpose { get; set; }
    public List<Guid> AccountIds { get; set; } = [];
    public bool? IsActive { get; set; }
    public string? SchoolYear { get; set; }
    public string? SchoolShift { get; set; }
    public List<string>? Content { get; set; }
    public List<Guid>? TeacherIds { get; set; }
    public List<Guid>? StudentIds { get; set; }
}

public class UpdateClassCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateClassCommand, Unit>
{
    public async Task<Unit> Handle(UpdateClassCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Classes
            .Include(c => c.AccountClasses)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        if (request.Name != null) entity.Name = request.Name;
        if (request.Description != null) entity.Description = request.Description;
        if (request.Purpose != null && !request.Purpose.Equals(ClassPurpose.None))
            entity.Purpose = request.Purpose.Value;
        if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        if (request.SchoolYear != null) entity.SchoolYear = request.SchoolYear;
        if (request.SchoolShift != null) entity.SchoolShift = request.SchoolShift;
        if (request.Content != null) entity.Content = request.Content;

        // Handle AccountClasses
        var allAccountClasses = context.AccountClasses
            .IgnoreQueryFilters()
            .Where(ac => ac.ClassId == entity.Id)
            .ToList();

        var currentAccountIds = allAccountClasses.Where(ac => !ac.IsDeleted).Select(ac => ac.AccountId).ToList();
        
        // Combina TeacherIds, StudentIds e AccountIds
        var newAccountIds = new List<Guid>();
        if (request.TeacherIds?.Any() == true)
            newAccountIds.AddRange(request.TeacherIds);
        if (request.StudentIds?.Any() == true)
            newAccountIds.AddRange(request.StudentIds);
        if (request.AccountIds?.Any() == true)
            newAccountIds.AddRange(request.AccountIds);
        
        newAccountIds = newAccountIds.Distinct().ToList();

        // Find accounts to add
        var accountsToAdd = newAccountIds.Except(currentAccountIds).ToList();
        foreach (var accountId in accountsToAdd)
        {
            var existingAccountClass =
                allAccountClasses.FirstOrDefault(ac => ac.AccountId == accountId && ac.IsDeleted);
            if (existingAccountClass != null)
            {
                existingAccountClass.IsDeleted = false;
                existingAccountClass.DeletedAt = null;
            }
            else
            {
                entity.AccountClasses.Add(new AccountClass { AccountId = accountId, ClassId = entity.Id });
            }
        }

        // Find accounts to remove (soft delete)
        var accountsToRemove = currentAccountIds.Except(newAccountIds).ToList();
        foreach (var accountClass in accountsToRemove.Select(accountId =>
                     entity.AccountClasses.First(ac => ac.AccountId == accountId)))
        {
            accountClass.IsDeleted = true;
            accountClass.DeletedAt = DateTimeOffset.UtcNow;
        }

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}