using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Item;

public record GetItemsByNamePaginatedQuery(string Name) : IRequest<PaginatedList<ItemDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetNpcByNamePaginatedQueryHandler : IRequestHandler<GetItemsByNamePaginatedQuery, PaginatedList<ItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetNpcByNamePaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ItemDto>> Handle(GetItemsByNamePaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Items
            .Where(n => n.Name.ToLower().Contains(request.Name.ToLower()))
            .OrderBy(n => n.Name)
            .ProjectTo<ItemDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}