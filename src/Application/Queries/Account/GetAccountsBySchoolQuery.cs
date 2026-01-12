using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Application.Queries.Account;

public record GetAccountsBySchoolQuery(Guid SchoolId) : IRequest<PaginatedList<AccountDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string SearchTerm { get; init; } = string.Empty;
}

public class GetAccountsBySchoolQueryHandler : IRequestHandler<GetAccountsBySchoolQuery, PaginatedList<AccountDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAccountsBySchoolQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<AccountDto>> Handle(GetAccountsBySchoolQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Accounts
            .Include(a => a.Client)
            .Where(a => a.AccountSchools.Any(accountSchool => accountSchool.SchoolId == request.SchoolId));

        // Aplicar filtro de busca se fornecido
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            query = query.Where(a =>
                (a.Name != null && a.Name.ToLower().Contains(searchLower)) ||
                (a.LastName != null && a.LastName.ToLower().Contains(searchLower)) ||
                (a.Email != null && a.Email.ToLower().Contains(searchLower)));
        }

        return await query
            .OrderBy(a => a.Name)
            .ProjectTo<AccountDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
