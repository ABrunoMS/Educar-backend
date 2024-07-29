using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.Proficiency;

public class GetProficiencyQuery : IRequest<ProficiencyDto>
{
    public Guid Id { get; init; }
}

public class GetGameQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetProficiencyQuery, ProficiencyDto>
{
    public async Task<ProficiencyDto> Handle(GetProficiencyQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.Proficiencies
            .Include(g => g.ProficiencyGroupProficiencies)
            .ThenInclude(gs => gs.ProficiencyGroup)
            .ProjectTo<ProficiencyDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.Game), request.Id.ToString());

        return mapper.Map<ProficiencyDto>(entity);
    }
}