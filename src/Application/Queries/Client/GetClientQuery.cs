using Educar.Backend.Application.Interfaces;

namespace Educar.Backend.Application.Queries.Client;

public class GetClientQuery : IRequest<ClientDto>
{
    public Guid Id { get; init; }
}

public class GetClientQueryHandler : IRequestHandler<GetClientQuery, ClientDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetClientQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ClientDto> Handle(GetClientQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Clients
            .ProjectTo<ClientDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.Client), request.Id.ToString());

        return _mapper.Map<ClientDto>(entity);
    }
}