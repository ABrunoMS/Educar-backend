using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Proficiency;

public record GetProficienciesPaginatedQuery : IRequest<PaginatedList<ProficiencyCleanDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetProficienciesPaginatedQueryHandler : IRequestHandler<GetProficienciesPaginatedQuery,
    PaginatedList<ProficiencyCleanDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProficienciesPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ProficiencyCleanDto>> Handle(GetProficienciesPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Proficiencies
            .OrderBy(x => x.Name)
            .ProjectTo<ProficiencyCleanDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}