using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Client;

public class GetClientsPaginatedQuery : IRequest<PaginatedList<ClientDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GeClientsPaginatedQueryHandler : IRequestHandler<GetClientsPaginatedQuery, PaginatedList<ClientDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GeClientsPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ClientDto>> Handle(GetClientsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Clients
            .OrderBy(x => x.Name)
            .ProjectTo<ClientDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}