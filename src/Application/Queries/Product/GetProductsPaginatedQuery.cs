using AutoMapper.QueryableExtensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Application.Queries.Product;


public record GetProductsPaginatedQuery : IRequest<PaginatedList<ProductDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    
}


public class GetProductsPaginatedQueryHandler : IRequestHandler<GetProductsPaginatedQuery, PaginatedList<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUser _currentUser;

    public GetProductsPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper, IUser currentUser)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<ProductDto>> Handle(GetProductsPaginatedQuery request, CancellationToken cancellationToken)
    {
        var userRoles = _currentUser.Roles ?? new List<string>();
        var adminRoleName = UserRole.Admin.ToString();
        
        // Se for Admin, retorna todos os produtos
        if (userRoles.Contains(adminRoleName))
        {
            return await _context.Products
                .OrderBy(p => p.Name)
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
        
        // Para outros usuários, filtrar pelos produtos vinculados ao cliente do usuário
        var userId = _currentUser.Id;
        Guid? clientId = null;
        
        if (!string.IsNullOrEmpty(userId))
        {
            var account = await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id.ToString() == userId, cancellationToken);
            clientId = account?.ClientId;
        }
        
        // Se o usuário não tem cliente associado, retorna lista vazia
        if (!clientId.HasValue)
        {
            return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, request.PageNumber, request.PageSize);
        }
        
        // Buscar os IDs dos produtos vinculados ao cliente
        var clientProductIds = await _context.ClientProducts
            .AsNoTracking()
            .Where(cp => cp.ClientId == clientId.Value)
            .Select(cp => cp.ProductId)
            .ToListAsync(cancellationToken);
        
        // Retornar apenas os produtos do cliente
        return await _context.Products
            .Where(p => clientProductIds.Contains(p.Id))
            .OrderBy(p => p.Name)
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}