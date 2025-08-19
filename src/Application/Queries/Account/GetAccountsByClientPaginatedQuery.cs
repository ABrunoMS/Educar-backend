using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;
using AutoMapper.QueryableExtensions; // Adicione este using

namespace Educar.Backend.Application.Queries.Account;

// A Query que recebe o ClientId e os parâmetros de paginação
public record GetAccountsByClientPaginatedQuery(Guid ClientId) : IRequest<PaginatedList<CleanAccountDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

// O Handler que executa a lógica de busca e filtro
public class GetAccountsByClientPaginatedQueryHandler : IRequestHandler<GetAccountsByClientPaginatedQuery, PaginatedList<CleanAccountDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAccountsByClientPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<CleanAccountDto>> Handle(GetAccountsByClientPaginatedQuery request, CancellationToken cancellationToken)
    {
        // 1. Busca na tabela de Contas (Accounts)
        // 2. Filtra pelo ClientId fornecido
        return await _context.Accounts
            .Where(a => a.ClientId == request.ClientId)
            .OrderBy(x => x.Name)
            .ProjectTo<CleanAccountDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}