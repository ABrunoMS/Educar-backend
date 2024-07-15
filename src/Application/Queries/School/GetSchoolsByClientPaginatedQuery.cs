using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.School;

public record GetSchoolsByClientPaginatedQuery(Guid ClientId) : IRequest<PaginatedList<SchoolDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetSchoolsByClientPaginatedQueryHandler : IRequestHandler<GetSchoolsByClientPaginatedQuery,
    PaginatedList<SchoolDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSchoolsByClientPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<SchoolDto>> Handle(GetSchoolsByClientPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Schools
            .OrderBy(x => x.Name)
            .Where(x => x.ClientId == request.ClientId)
            .ProjectTo<SchoolDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}