using AutoMapper.QueryableExtensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Queries.Content; // Usando o ContentDto
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Application.Queries.Product;


public record GetCompatibleContentsQuery(Guid ProductId) : IRequest<List<ContentDto>>;


public class GetCompatibleContentsQueryHandler : IRequestHandler<GetCompatibleContentsQuery, List<ContentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCompatibleContentsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ContentDto>> Handle(GetCompatibleContentsQuery request, CancellationToken cancellationToken)
    {
        // 1. Acessa a tabela de ligação 'ProductContents'
        // 2. Filtra pelo ProductId que recebemos
        // 3. Seleciona a entidade 'Content' relacionada
        // 4. Mapeia para 'ContentDto'
        return await _context.ProductContents
            .Where(pc => pc.ProductId == request.ProductId)
            .Select(pc => pc.Content)
            .ProjectTo<ContentDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}