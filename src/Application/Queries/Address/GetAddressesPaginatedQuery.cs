using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Address;

public record GetAddressesPaginatedQuery : IRequest<PaginatedList<AddressDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetAddressesPaginatedQueryHandler : IRequestHandler<GetAddressesPaginatedQuery, PaginatedList<AddressDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAddressesPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<AddressDto>> Handle(GetAddressesPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Addresses
            .OrderBy(x => x.Street)
            .ProjectTo<AddressDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
