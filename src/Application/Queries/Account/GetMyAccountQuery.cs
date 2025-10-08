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
        // Pega o ID do usuário logado (que é uma string)
        var userIdString = _currentUserService.Id;
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException();
        }

        // CORREÇÃO: Convertemos a string para Guid antes de comparar
        var userIdGuid = Guid.Parse(userIdString);

        var entity = await _context.Accounts
            .Include(a => a.AccountSchools)
                .ThenInclude(asc => asc.School)
            .Include(a => a.AccountClasses)
                .ThenInclude(ac => ac.Class)
            // Usamos o Guid convertido na comparação
            .FirstOrDefaultAsync(e => e.Id == userIdGuid, cancellationToken);

        Guard.Against.NotFound(userIdGuid, entity);

        return _mapper.Map<AccountDto>(entity);
    }
}