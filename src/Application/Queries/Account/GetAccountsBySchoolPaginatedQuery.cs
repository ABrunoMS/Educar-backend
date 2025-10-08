using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Account;

public record GetAccountsBySchoolPaginatedQuery(Guid SchoolId) : IRequest<PaginatedList<CleanAccountDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Search { get; init; }
}

public class GetAccountsBySchoolPaginatedQueryHandler : IRequestHandler<GetAccountsBySchoolPaginatedQuery,
    PaginatedList<CleanAccountDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAccountsBySchoolPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<CleanAccountDto>> Handle(GetAccountsBySchoolPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.AccountSchools
        .Where(asc => asc.SchoolId == request.SchoolId)
        .Select(asc => asc.Account);

        if (!string.IsNullOrEmpty(request.Search))
        {
            // Filtra por nome OU email que contenham o texto da busca (ignorando maiúsculas/minúsculas)
            query = query.Where(a => 
                a.Name.ToLower().Contains(request.Search.ToLower())
                );
        }
        return await _context.AccountSchools
            .Where(asc => asc.SchoolId == request.SchoolId)
            .Select(asc => asc.Account) 
            .OrderBy(x => x.Name)
            .ProjectTo<CleanAccountDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}