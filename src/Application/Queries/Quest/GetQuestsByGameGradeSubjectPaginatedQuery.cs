using AutoMapper;
using AutoMapper.QueryableExtensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Educar.Backend.Application.Queries.Quest;

public class GetQuestsByGameGradeSubjectPaginatedQuery : IRequest<PaginatedList<QuestCleanDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public Guid? GameId { get; init; }
    public Guid? GradeId { get; init; }
    public Guid? SubjectId { get; init; }
}

public class GetQuestsByGameGradeSubjectPaginatedQueryHandler : IRequestHandler<GetQuestsByGameGradeSubjectPaginatedQuery,
    PaginatedList<QuestCleanDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUser _currentUser;
    private readonly ILogger<GetQuestsByGameGradeSubjectPaginatedQueryHandler> _logger;

    public GetQuestsByGameGradeSubjectPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper, IUser currentUser, ILogger<GetQuestsByGameGradeSubjectPaginatedQueryHandler> logger)
        
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<PaginatedList<QuestCleanDto>> Handle(GetQuestsByGameGradeSubjectPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Quests.AsNoTracking();

        
        // Pega o cargo e o ID do usuário logado (através do token JWT)
        var userRoles =_currentUser.Roles ?? new List<string>();
        var userIdString = _currentUser.Id;
        var teacherRoleName = UserRole.Teacher.ToString(); 
        var adminRoleName = UserRole.Admin.ToString();

        _logger.LogWarning("--- INICIANDO DEPURAÇÃO DE FILTRO DE QUESTS ---");
    _logger.LogWarning("Cargos vindos do Token (IUser.Roles): {ActualRoles}", string.Join(", ", userRoles));
    _logger.LogWarning("UserID vindo do Token (IUser.Id): {UserId}", userIdString);
    _logger.LogWarning("--------------------------------------------------");

    // Se o usuário for um "Teacher" E NÃO for um "Admin"
    if (userRoles.Contains(teacherRoleName) && !userRoles.Contains(adminRoleName))
    {
            if (string.IsNullOrEmpty(userIdString))
            {
                _logger.LogError("Usuário é Teacher, mas o ID (UserID) está nulo ou vazio. Filtro não aplicado.");
            }
            else
            {
          _logger.LogInformation("CONDIÇÃO (IF) VERDADEIRA: Usuário é Teacher (mas não Admin). Aplicando filtro.");
                
                // --- 2. CORREÇÃO PRINCIPAL ---
                // Compara a propriedade 'CreatedBy' (string) com o 'userIdString' (string)
        query = query.Where(q => q.CreatedBy == userIdString); 
            }
    }
    else if (userRoles.Contains(adminRoleName))
    {
      _logger.LogWarning("CONDIÇÃO (IF) FALSA: Usuário é Admin. O filtro de Teacher NÃO será aplicado.");
    }
    else
    {
      _logger.LogWarning("CONDIÇÃO (IF) FALSA: Usuário não é Teacher nem Admin. O filtro de Teacher NÃO será aplicado.");
    }

        // if (request.GameId is not null)
        // {
        //     query = query.Where(q => q.GameId == request.GameId);
        // }

        // if (request.GradeId is not null)
        // {
        //     query = query.Where(q => q.GradeId == request.GradeId);
        // }

        // if (request.SubjectId is not null)
        // {
        //     query = query.Where(q => q.SubjectId == request.SubjectId);
        // }

        return await query
            .OrderBy(q => q.Name)
            .ProjectTo<QuestCleanDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}