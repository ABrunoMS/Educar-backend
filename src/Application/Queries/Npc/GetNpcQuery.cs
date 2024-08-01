using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.Npc;

public class GetNpcQuery : IRequest<NpcDto>
{
    public Guid Id { get; init; }
}

public class GetNpcQueryHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetNpcQuery, NpcDto>
{
    public async Task<NpcDto> Handle(GetNpcQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.Npcs
            .Include(g => g.NpcItems)
            .ThenInclude(gs => gs.Item)
            .Include(g => g.Dialogues)
            .Include(g => g.GameNpcs)
            .ThenInclude(gs => gs.Game)
            .ProjectTo<NpcDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.Npc), request.Id.ToString());

        var npcDto = mapper.Map<NpcDto>(entity);

        // Order the Dialogues by Order
        if (npcDto.Dialogues != null) npcDto.Dialogues = npcDto.Dialogues.OrderBy(d => d.Order).ToList();

        return npcDto;
    }
}