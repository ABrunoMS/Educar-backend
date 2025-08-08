using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.ProficiencyGroup;

public class GetProficiencyGroupQuery : IRequest<ProficiencyGroupDto>
{
    public Guid Id { get; init; }
}

public class GetProficiencyGroupQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetProficiencyGroupQuery, ProficiencyGroupDto>
{
    public async Task<ProficiencyGroupDto> Handle(GetProficiencyGroupQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.ProficiencyGroups
            .Include(g => g.ProficiencyGroupProficiencies)
            .ThenInclude(gs => gs.Proficiency)
            .ProjectTo<ProficiencyGroupDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(Domain.Entities.ProficiencyGroup), request.Id.ToString());

        return mapper.Map<ProficiencyGroupDto>(entity);
    }
}