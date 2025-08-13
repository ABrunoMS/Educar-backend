using AutoMapper.QueryableExtensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Client;

// ADICIONADO: Propriedades para Search e Filter
public class GetClientsPaginatedQuery : IRequest<PaginatedList<ClientDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Search { get; init; }
    public string? Filter { get; init; }
    public string? SortBy { get; init; }
    public string? SortOrder { get; init; }
}

// CORRIGIDO: Typo no nome da classe e implementação da lógica
public class GetClientsPaginatedQueryHandler : IRequestHandler<GetClientsPaginatedQuery, PaginatedList<ClientDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetClientsPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ClientDto>> Handle(GetClientsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<Domain.Entities.Client> query = _context.Clients.AsNoTracking();

        // LÓGICA DE PESQUISA (SEARCH) ADICIONADA
        if (!string.IsNullOrEmpty(request.Search))
        {
            // Busca pelo nome, ignorando maiúsculas/minúsculas
            query = query.Where(c => c.Name.ToLower().Contains(request.Search.ToLower()));
        }

        // LÓGICA DE ORDENAÇÃO (SORTING) ADICIONADA
        // Nota: Isso é um exemplo simples. Uma implementação robusta usaria Expressions.
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            bool isDescending = request.SortOrder?.ToLower() == "desc";
            query = request.SortBy.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "totalaccounts" => isDescending ? query.OrderByDescending(x => x.TotalAccounts) : query.OrderBy(x => x.TotalAccounts),
                _ => query.OrderBy(x => x.Name) // Ordenação padrão
            };
        }
        else
        {
            query = query.OrderBy(x => x.Name); // Ordenação padrão
        }

        return await query
            .ProjectTo<ClientDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}