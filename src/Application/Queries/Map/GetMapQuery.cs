using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Queries.Npc;

namespace Educar.Backend.Application.Queries.Map;

public class GetMapQuery : IRequest<MapDto>
{
    public Guid Id { get; init; }
}

public class GetMapQueryHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetMapQuery, MapDto>
{
    public async Task<MapDto> Handle(GetMapQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.Maps
            .Include(g => g.SpawnPoints)
            .ProjectTo<MapDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.Map), request.Id.ToString());

        return mapper.Map<MapDto>(entity);
    }
}