using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.Game;

public class GetGameQuery : IRequest<GameDto>
{
    public Guid Id { get; init; }
}

public class GetGameQueryHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetGameQuery, GameDto>
{
    public async Task<GameDto> Handle(GetGameQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.Games
            .Include(g => g.GameSubjects)
            .ThenInclude(gs => gs.Subject)
            .Include(g => g.GameProficiencyGroups)
            .ThenInclude(gp => gp.ProficiencyGroup)
            .Include(g => g.GameNpcs)
            .ThenInclude(gn => gn.Npc)
            .ProjectTo<GameDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.Game), request.Id.ToString());

        return mapper.Map<GameDto>(entity);
    }
}