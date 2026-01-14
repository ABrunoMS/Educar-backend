using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums; // Adicione este using para UserRole
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Educar.Backend.Application.Queries.Class;


public record GetClassesPaginatedQuery : IRequest<PaginatedList<ClassDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}


public class GetClassesPaginatedQueryHandler : IRequestHandler<GetClassesPaginatedQuery, PaginatedList<ClassDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUser _currentUserService;
    private readonly ILogger<GetClassesPaginatedQueryHandler> _logger;

    public GetClassesPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper, IUser currentUserService, ILogger<GetClassesPaginatedQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<PaginatedList<ClassDto>> Handle(GetClassesPaginatedQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.Id;
        if (currentUserId == null)
        {
            // Usa o novo método Create para retornar uma lista vazia
            return PaginatedList<ClassDto>.Create(new List<ClassDto>(), 0, request.PageNumber, request.PageSize);
        }

        IQueryable<Domain.Entities.Class> query;

        _logger.LogInformation("--- GetClassesPaginatedQueryHandler ---");
        _logger.LogInformation("Checking user with ID: {UserId}", _currentUserService.Id);
        _logger.LogInformation("User has Roles: {UserRoles}", _currentUserService.Roles != null ? string.Join(", ", _currentUserService.Roles) : "null");
        _logger.LogInformation("Checking if roles contain 'Admin': {ContainsAdmin}", _currentUserService.Roles?.Contains(UserRole.Admin.ToString()));
        _logger.LogInformation("-------------------------------------");

        // CORREÇÃO 2: Verificando se a lista de Roles contém "Admin"
        if (_currentUserService.Roles != null && _currentUserService.Roles.Contains(UserRole.Admin.ToString()))
        {
            // SE FOR ADMIN: Busca todas as turmas
            query = _context.Classes
                .Include(c => c.ClassProducts)
                    .ThenInclude(cp => cp.Product)
                .Include(c => c.ClassContents)
                    .ThenInclude(cc => cc.Content)
                .Include(c => c.AccountClasses)
                    .ThenInclude(ac => ac.Account)
                .AsQueryable();
        }
        else // PARA TEACHER, STUDENT, ETC.
        {
            var userSchoolIdsQuery = _context.AccountSchools
                .Where(asc => asc.AccountId == currentUserId.Value)
                .Select(asc => asc.SchoolId);
            
            query = _context.Classes
                .Include(c => c.ClassProducts)
                    .ThenInclude(cp => cp.Product)
                .Include(c => c.ClassContents)
                    .ThenInclude(cc => cc.Content)
                .Include(c => c.AccountClasses)
                    .ThenInclude(ac => ac.Account)
                .Where(c => userSchoolIdsQuery.Contains(c.SchoolId));
        }

        return await query
            .OrderBy(c => c.Name)
            .ProjectTo<ClassDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}