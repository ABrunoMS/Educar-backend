using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.Account;

public class GetAccountQuery : IRequest<AccountDto>
{
    public Guid Id { get; init; }
}

public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAccountQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AccountDto> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Accounts
            .Include(a => a.School)
            .Include(a => a.AccountClasses)
            .ThenInclude(ac => ac.Class)
            .ProjectTo<AccountDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.Account), request.Id.ToString());

        return _mapper.Map<AccountDto>(entity);
    }
}