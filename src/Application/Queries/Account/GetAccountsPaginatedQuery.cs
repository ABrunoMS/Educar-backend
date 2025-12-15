using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.Account;

public record GetAccountsPaginatedQuery : IRequest<PaginatedList<CleanAccountDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Role { get; init; }
}

public class GetAccountsPaginatedQueryHandler : IRequestHandler<GetAccountsPaginatedQuery, PaginatedList<CleanAccountDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAccountsPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<CleanAccountDto>> Handle(GetAccountsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Accounts.AsQueryable();

        // Filtro por Role
        if (!string.IsNullOrEmpty(request.Role) && Enum.TryParse<UserRole>(request.Role, true, out var roleEnum))
        {
            query = query.Where(a => a.Role == roleEnum);
        }

        return await query
            .OrderBy(x => x.Email)
            .ProjectTo<CleanAccountDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}