using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Grade;

public class GetGradesPaginatedQuery : IRequest<PaginatedList<GradeDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetGradesPaginatedQueryHandler : IRequestHandler<GetGradesPaginatedQuery, PaginatedList<GradeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGradesPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<GradeDto>> Handle(GetGradesPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Grades
            .OrderBy(x => x.Name)
            .ProjectTo<GradeDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}