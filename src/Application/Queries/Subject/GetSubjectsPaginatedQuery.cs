using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Subject;

public class GetSubjectsPaginatedQuery : IRequest<PaginatedList<SubjectDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetSubjectsPaginatedQueryHandler : IRequestHandler<GetSubjectsPaginatedQuery, PaginatedList<SubjectDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSubjectsPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<SubjectDto>> Handle(GetSubjectsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Subjects
            .OrderBy(x => x.Name)
            .ProjectTo<SubjectDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}