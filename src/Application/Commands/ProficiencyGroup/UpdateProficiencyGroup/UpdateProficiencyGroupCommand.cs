using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Application.Commands.ProficiencyGroup.UpdateProficiencyGroup;

public record UpdateProficiencyGroupCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public IList<Guid> ProficiencyIds { get; set; } = new List<Guid>();
}

public class UpdateProficiencyGroupCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateProficiencyGroupCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProficiencyGroupCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.ProficiencyGroups
            .Include(g => g.ProficiencyGroupProficiencies)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        if (request.Name != null) entity.Name = request.Name;
        if (request.Description != null) entity.Description = request.Description;

        // Validate and fetch proficiencies
        var proficiencyEntities = await context.Proficiencies
            .Where(s => request.ProficiencyIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        var missingIds = request.ProficiencyIds.Except(proficiencyEntities.Select(s => s.Id)).ToList();
        if (missingIds.Any())
        {
            throw new NotFoundException(nameof(Domain.Entities.Proficiency), string.Join(", ", missingIds));
        }

        // Handle relationships
        var allProficiencyGroupProficiencies = await context.ProficiencyGroupProficiencies
            .IgnoreQueryFilters()
            .Where(gs => gs.ProficiencyGroupId == entity.Id)
            .ToListAsync(cancellationToken);

        var currentIds = allProficiencyGroupProficiencies.Where(gs => !gs.IsDeleted).Select(gs => gs.ProficiencyId)
            .ToList();
        var newIds = request.ProficiencyIds;

        // Find proficiencies to add
        var proficienciesToAdd = newIds.Except(currentIds).ToList();
        foreach (var proficiencyId in proficienciesToAdd)
        {
            var existingProficiencyGroupProficiencies =
                allProficiencyGroupProficiencies.FirstOrDefault(gs =>
                    gs.ProficiencyId == proficiencyId && gs.IsDeleted);
            if (existingProficiencyGroupProficiencies != null)
            {
                existingProficiencyGroupProficiencies.IsDeleted = false;
                existingProficiencyGroupProficiencies.DeletedAt = null;
            }
            else
            {
                entity.ProficiencyGroupProficiencies.Add(new ProficiencyGroupProficiency
                    { ProficiencyGroupId = entity.Id, ProficiencyId = proficiencyId });
            }
        }

        // Find proficiencies to remove (soft delete)
        var proficienciesToRemove = currentIds.Except(newIds).ToList();
        foreach (var proficiencyGroupProficiency in proficienciesToRemove.Select(proficiencyId => allProficiencyGroupProficiencies.FirstOrDefault(gs => gs.ProficiencyId == proficiencyId)).OfType<ProficiencyGroupProficiency>())
        {
            proficiencyGroupProficiency.IsDeleted = true;
            proficiencyGroupProficiency.DeletedAt = DateTimeOffset.UtcNow;
        }

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}