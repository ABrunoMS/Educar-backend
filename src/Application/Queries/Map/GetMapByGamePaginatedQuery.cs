using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Map;

public record GetMapByGamePaginatedQuery(Guid GameId) : IRequest<PaginatedList<MapCleanDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetMapByGamePaginatedQueryHandler : IRequestHandler<GetMapByGamePaginatedQuery,
    PaginatedList<MapCleanDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMapByGamePaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<MapCleanDto>> Handle(GetMapByGamePaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Maps
            .Where(x => x.GameId == request.GameId)
            .ProjectTo<MapCleanDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}