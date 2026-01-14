using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.Account;

// Esta query não precisa de parâmetros, pois pegará o ID do usuário logado
public record GetMyAccountQuery : IRequest<AccountDto>;

public class GetMyAccountQueryHandler : IRequestHandler<GetMyAccountQuery, AccountDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUser _currentUserService; // Serviço que obtém o usuário atual

    public GetMyAccountQueryHandler(IApplicationDbContext context, IMapper mapper, IUser currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<AccountDto> Handle(GetMyAccountQuery request, CancellationToken cancellationToken)
    {
        // Pega o ID do usuário logado (que agora é um Guid?)
        var userId = _currentUserService.Id;
        if (userId == null)
        {
            throw new UnauthorizedAccessException();
        }

        var entity = await _context.Accounts
            .Include(a => a.AccountSchools)
                .ThenInclude(asc => asc.School)
            .Include(a => a.AccountClasses)
                .ThenInclude(ac => ac.Class)
            // Usamos o Guid diretamente na comparação
            .FirstOrDefaultAsync(e => e.Id == userId.Value, cancellationToken);

        Guard.Against.NotFound(userId.Value, entity);

        return _mapper.Map<AccountDto>(entity);
    }
}