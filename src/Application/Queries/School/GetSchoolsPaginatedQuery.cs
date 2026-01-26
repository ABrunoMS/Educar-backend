using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.School;

public class GetSchoolsPaginatedQuery : IRequest<PaginatedList<SchoolDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Search { get; init; }
    public string? Filter { get; init; }
    public string? SortBy { get; init; }
    public string? SortOrder { get; init; }
    public Guid? ClientId { get; init; }
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
        var query = _context.Schools.AsQueryable();

        // Aplicar filtro por Cliente
        if (request.ClientId.HasValue)
        {
            query = query.Where(s => s.ClientId == request.ClientId.Value);
        }

        // Aplicar busca por nome
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(s => s.Name.ToLower().Contains(request.Search.ToLower()));
        }

        // Aplicar ordenação
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            bool isDescending = request.SortOrder?.ToLower() == "desc";
            query = request.SortBy.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "description" => isDescending ? query.OrderByDescending(x => x.Description) : query.OrderBy(x => x.Description),
                _ => query.OrderBy(x => x.Name)
            };
        }
        else
        {
            query = query.OrderBy(x => x.Name);
        }

        return await query
            .ProjectTo<SchoolDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}