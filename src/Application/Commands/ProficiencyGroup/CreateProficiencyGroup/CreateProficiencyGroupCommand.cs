using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Application.Commands.ProficiencyGroup.CreateProficiencyGroup;

public class CreateProficiencyGroupCommand(string Name, string Description)
    : IRequest<IdResponseDto>
{
    public string Name { get; set; } = Name;
    public string Description { get; set; } = Description;
    public IList<Guid> ProficiencyIds { get; set; } = new List<Guid>();
}

public class CreateProficiencyGroupCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateProficiencyGroupCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateProficiencyGroupCommand request,
        CancellationToken cancellationToken)
    {
        var proficiencyEntities = new List<Domain.Entities.Proficiency>();
        if (request.ProficiencyIds != null && request.ProficiencyIds.Count != 0)
        {
            proficiencyEntities = await context.Proficiencies
                .Where(c => request.ProficiencyIds.Contains(c.Id))
                .ToListAsync(cancellationToken);

            var missingIds = request.ProficiencyIds.Except(proficiencyEntities.Select(c => c.Id)).ToList();
            if (missingIds.Count != 0)
            {
                throw new NotFoundException(nameof(Domain.Entities.Proficiency), string.Join(", ", missingIds));
            }
        }

        var entity = new Domain.Entities.ProficiencyGroup(request.Name, request.Description);

        foreach (var proficiency in proficiencyEntities)
        {
            proficiency.ProficiencyGroupProficiencies.Add(new ProficiencyGroupProficiency
                { ProficiencyGroup = entity, Proficiency = proficiency });
        }

        context.ProficiencyGroups.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}