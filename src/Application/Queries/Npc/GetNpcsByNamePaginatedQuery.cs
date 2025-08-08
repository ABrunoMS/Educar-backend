using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Npc;

public record GetNpcsByNamePaginatedQuery(string Name) : IRequest<PaginatedList<NpcCleanDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class
    GetNpcsByNamePaginatedQueryHandler : IRequestHandler<GetNpcsByNamePaginatedQuery, PaginatedList<NpcCleanDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetNpcsByNamePaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<NpcCleanDto>> Handle(GetNpcsByNamePaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Npcs
            .Include(n => n.NpcItems)
            .ThenInclude(ni => ni.Item)
            .Include(n => n.Dialogues)
            .Where(n => n.Name.ToLower().Contains(request.Name.ToLower()))
            .OrderBy(n => n.Name)
            .ProjectTo<NpcCleanDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}