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
    public string? Search { get; init; }
    public Guid? ClientId { get; init; }
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
        var query = _context.Accounts
            .Include(a => a.Client)
            .AsQueryable();

        // Filtro por Cliente
        if (request.ClientId.HasValue)
        {
            query = query.Where(a => a.ClientId == request.ClientId.Value);
        }

        // Filtro por Role
        if (!string.IsNullOrEmpty(request.Role) && Enum.TryParse<UserRole>(request.Role, true, out var roleEnum))
        {
            query = query.Where(a => a.Role == roleEnum);
        }

        // Busca por nome ou email
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(a => a.Name.ToLower().Contains(request.Search.ToLower()) || 
                                    a.Email.ToLower().Contains(request.Search.ToLower()));
        }

        return await query
            .OrderBy(x => x.Email)
            .ProjectTo<CleanAccountDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}