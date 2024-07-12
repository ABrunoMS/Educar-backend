using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Game;

public record GetGamesPaginatedQuery : IRequest<PaginatedList<GameDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetGamesPaginatedQueryHandler : IRequestHandler<GetGamesPaginatedQuery, PaginatedList<GameDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGamesPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<GameDto>> Handle(GetGamesPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Games
            .OrderBy(x => x.Name)
            .ProjectTo<GameDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}