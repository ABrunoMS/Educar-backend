using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Npc;

public record GetNpcsByNamePaginatedQuery(string Name) : IRequest<PaginatedList<NpcDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetNpcsByNamePaginatedQueryHandler : IRequestHandler<GetNpcsByNamePaginatedQuery, PaginatedList<NpcDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetNpcsByNamePaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<NpcDto>> Handle(GetNpcsByNamePaginatedQuery request,
        CancellationToken cancellationToken)
    {
        // Fetch the Npc entities with their related items and dialogues
        var query = _context.Npcs
            .Include(n => n.NpcItems)
            .ThenInclude(ni => ni.Item)
            .Include(n => n.Dialogues)
            .Where(n => n.Name.ToLower().Contains(request.Name.ToLower()))
            .OrderBy(n => n.Name)
            .AsQueryable();

        // Fetch paginated data
        var paginatedEntities =
            await PaginatedList<Domain.Entities.Npc>.CreateAsync(query, request.PageNumber, request.PageSize);

        // Map entities to DTOs
        var npcDtos = _mapper.Map<List<NpcDto>>(paginatedEntities.Items);

        // Order dialogues within each NpcDto
        foreach (var npcDto in npcDtos)
        {
            if (npcDto.Dialogues != null) npcDto.Dialogues = npcDto.Dialogues.OrderBy(d => d.Order).ToList();
        }

        // Return paginated list of NpcDto
        return new PaginatedList<NpcDto>(npcDtos, paginatedEntities.TotalCount, request.PageNumber,
            request.PageSize);
    }
}