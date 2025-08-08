using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Class;

public record GetClassesBySchoolPaginatedQuery(Guid SchoolId) : IRequest<PaginatedList<ClassDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetClassesBySchoolPaginatedQueryHandler : IRequestHandler<GetClassesBySchoolPaginatedQuery,
    PaginatedList<ClassDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetClassesBySchoolPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ClassDto>> Handle(GetClassesBySchoolPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Classes
            .OrderBy(x => x.Name)
            .Where(x => x.SchoolId == request.SchoolId)
            .ProjectTo<ClassDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}