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
    public Guid? ProductId { get; init; }
    public Guid? ContentId { get; init; }
    public string? Search { get; init; }
    public bool UsageTemplate { get; init; }
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
        // Obter o ClientId do usuário atual
        var userId = _currentUser.Id;
        Guid? clientId = null;
        
        if (!string.IsNullOrEmpty(userId))
        {
            var account = await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id.ToString() == userId, cancellationToken);
            clientId = account?.ClientId;
        }

        IQueryable<Domain.Entities.Quest> query = _context.Quests
          .AsNoTracking()
          .Include(q => q.Subject) 
          .Include(q => q.Grade)
          .Include(q => q.Content)
          .Include(q => q.Product);

        
        
        query = query.Where(q => q.UsageTemplate == request.UsageTemplate);
        

        
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
            
            bool isSearchingTemplates = request.UsageTemplate == true;

            if (!isSearchingTemplates) 
            {
                // Só aplica o filtro de dono se NÃO for uma busca por templates
                if (!string.IsNullOrEmpty(userIdString))
                {
                    query = query.Where(q => q.CreatedBy == userIdString);
                }
            }
        }

        // Filtrar por Content e Product que o cliente possui
        if (clientId.HasValue)
        {
            var clientContentIds = await _context.ClientContents
                .AsNoTracking()
                .Where(cc => cc.ClientId == clientId.Value)
                .Select(cc => cc.ContentId)
                .ToListAsync(cancellationToken);

            var clientProductIds = await _context.ClientProducts
                .AsNoTracking()
                .Where(cp => cp.ClientId == clientId.Value)
                .Select(cp => cp.ProductId)
                .ToListAsync(cancellationToken);

            // Retorna apenas quests que o cliente possui tanto o Content quanto o Product
            query = query.Where(q => 
                clientContentIds.Contains(q.ContentId) &&
                clientProductIds.Contains(q.ProductId)
            );
        }
        else
        {
            // Se não há cliente, não retorna nenhuma quest
            query = query.Where(q => false);
        }

        if (request.GameId is not null)
        {
            query = query.Where(q => q.GameId == request.GameId);
        }

        if (request.GradeId is not null)
        {
            query = query.Where(q => q.GradeId == request.GradeId);
        }

        if (request.SubjectId is not null)
        {
            query = query.Where(q => q.SubjectId == request.SubjectId);
        }

        if (request.ProductId is not null)
        {
            query = query.Where(q => q.ProductId == request.ProductId);
        }

        if (request.ContentId is not null)
        {
            query = query.Where(q => q.ContentId == request.ContentId);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search.ToLower();
            query = query.Where(q => q.Name.ToLower().Contains(searchLower));
        }

        return await query
            .OrderBy(q => q.Name)
            .ProjectTo<QuestCleanDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
