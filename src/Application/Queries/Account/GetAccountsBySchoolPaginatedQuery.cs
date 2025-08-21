using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Account;

public record GetAccountsBySchoolPaginatedQuery(Guid SchoolId) : IRequest<PaginatedList<CleanAccountDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
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
        return await _context.AccountSchools
            .Where(asc => asc.SchoolId == request.SchoolId)
            .Select(asc => asc.Account) 
            .OrderBy(x => x.Name)
            .ProjectTo<CleanAccountDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}