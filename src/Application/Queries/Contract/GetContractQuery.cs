using Educar.Backend.Application.Interfaces;

namespace Educar.Backend.Application.Queries.Contract;

public record GetContractQuery : IRequest<ContractDto>
{
    public Guid Id { get; init; }
}

public class GetContractQueryHandler : IRequestHandler<GetContractQuery, ContractDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetContractQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ContractDto> Handle(GetContractQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Contracts
            .ProjectTo<ContractDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.Contract), request.Id.ToString());

        return _mapper.Map<ContractDto>(entity);
    }
}