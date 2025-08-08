using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.ProficiencyGroup;

public class GetProficiencyGroupsPaginatedQuery : IRequest<PaginatedList<ProficiencyGroupCleanDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetProficiencyGroupsPaginatedQueryHandler : IRequestHandler<GetProficiencyGroupsPaginatedQuery,
    PaginatedList<ProficiencyGroupCleanDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProficiencyGroupsPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ProficiencyGroupCleanDto>> Handle(GetProficiencyGroupsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.ProficiencyGroups
            .OrderBy(x => x.Name)
            .ProjectTo<ProficiencyGroupCleanDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}