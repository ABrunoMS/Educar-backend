using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Contract;

public class GetContractsPaginatedQuery : IRequest<PaginatedList<ContractDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GeContractsPaginatedQueryHandler : IRequestHandler<GetContractsPaginatedQuery, PaginatedList<ContractDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GeContractsPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ContractDto>> Handle(GetContractsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Contracts
            .OrderBy(x => x.Id)
            .ProjectTo<ContractDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}