using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.School;

public class GetSchoolsPaginatedQuery : IRequest<PaginatedList<SchoolDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetSchoolsPaginatedQueryHandler : IRequestHandler<GetSchoolsPaginatedQuery, PaginatedList<SchoolDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSchoolsPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<SchoolDto>> Handle(GetSchoolsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Schools
            .OrderBy(x => x.Id)
            .ProjectTo<SchoolDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}