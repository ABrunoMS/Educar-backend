using AutoMapper;
using AutoMapper.QueryableExtensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

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
        // Buscar todos os clientes primeiro
        IQueryable<Domain.Entities.Client> clientQuery = _context.Clients
            .Include(c => c.ClientProducts)
                .ThenInclude(cp => cp.Product)
            .Include(c => c.ClientContents)
                .ThenInclude(cc => cc.Content)
            .Include(c => c.Subsecretarias)
            .Include(c => c.Accounts)
            .AsNoTracking();

        // LÓGICA DE PESQUISA (SEARCH) ADICIONADA
        if (!string.IsNullOrEmpty(request.Search))
        {
            clientQuery = clientQuery.Where(c => c.Name.ToLower().Contains(request.Search.ToLower()));
        }

        // LÓGICA DE ORDENAÇÃO (SORTING) ADICIONADA
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            bool isDescending = request.SortOrder?.ToLower() == "desc";
            clientQuery = request.SortBy.ToLower() switch
            {
                "name" => isDescending ? clientQuery.OrderByDescending(x => x.Name) : clientQuery.OrderBy(x => x.Name),
                "totalaccounts" => isDescending ? clientQuery.OrderByDescending(x => x.TotalAccounts) : clientQuery.OrderBy(x => x.TotalAccounts),
                _ => clientQuery.OrderBy(x => x.Name)
            };
        }
        else
        {
            clientQuery = clientQuery.OrderBy(x => x.Name);
        }

        var totalCount = await clientQuery.CountAsync(cancellationToken);

        // Aplicar paginação
        var clients = await clientQuery
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Buscar os IDs dos parceiros e contatos que são GUIDs válidos
        var accountIds = new List<Guid>();
        
        foreach (var client in clients)
        {
            if (!string.IsNullOrEmpty(client.Partner) && Guid.TryParse(client.Partner, out var partnerId))
            {
                accountIds.Add(partnerId);
            }
            if (!string.IsNullOrEmpty(client.Contacts) && Guid.TryParse(client.Contacts, out var contactId))
            {
                accountIds.Add(contactId);
            }
        }

        // Buscar os nomes das contas em uma única query
        var accounts = await _context.Accounts
            .AsNoTracking()
            .Where(a => accountIds.Contains(a.Id))
            .Select(a => new { a.Id, a.Name, a.LastName })
            .ToDictionaryAsync(a => a.Id.ToString(), a => $"{a.Name} {a.LastName}", cancellationToken);

        // Mapear para DTOs e adicionar os nomes
        var clientDtos = clients.Select(client =>
        {
            var dto = _mapper.Map<ClientDto>(client);
            
            // Buscar o nome do parceiro
            if (!string.IsNullOrEmpty(client.Partner) && accounts.TryGetValue(client.Partner, out var partnerName))
            {
                dto.PartnerName = partnerName;
            }
            else if (!string.IsNullOrEmpty(client.Partner) && !Guid.TryParse(client.Partner, out _))
            {
                // Se não for GUID, pode ser um texto direto
                dto.PartnerName = client.Partner;
            }
            
            // Buscar o nome do contato
            if (!string.IsNullOrEmpty(client.Contacts) && accounts.TryGetValue(client.Contacts, out var contactName))
            {
                dto.Contacts = contactName;
            }
            else if (!string.IsNullOrEmpty(client.Contacts) && !Guid.TryParse(client.Contacts, out _))
            {
                // Se não for GUID, manter o texto original
                // dto.Contacts já está mapeado pelo AutoMapper
            }
            
            return dto;
        }).ToList();

        return new PaginatedList<ClientDto>(
            clientDtos,
            totalCount,
            request.PageNumber,
            request.PageSize
        );
    }
}