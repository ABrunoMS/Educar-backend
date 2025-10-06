using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums; // Adicione este using para UserRole
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

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

    public GetClassesPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper, IUser currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<ClassDto>> Handle(GetClassesPaginatedQuery request, CancellationToken cancellationToken)
    {
        var currentUserIdString = _currentUserService.Id;
        if (string.IsNullOrEmpty(currentUserIdString))
        {
            // Usa o novo método Create para retornar uma lista vazia
            return PaginatedList<ClassDto>.Create(new List<ClassDto>(), 0, request.PageNumber, request.PageSize);
        }

        IQueryable<Domain.Entities.Class> query;

        // CORREÇÃO 2: Verificando se a lista de Roles contém "Admin"
        if (_currentUserService.Roles != null && _currentUserService.Roles.Contains(UserRole.Admin.ToString()))
        {
            // SE FOR ADMIN: Busca todas as turmas
            query = _context.Classes.AsQueryable();
        }
        else // PARA TEACHER, STUDENT, ETC.
        {
            // CORREÇÃO 3: Convertendo a string do ID para Guid
            var currentUserId = Guid.Parse(currentUserIdString);

            var userSchoolIdsQuery = _context.AccountSchools
                .Where(asc => asc.AccountId == currentUserId)
                .Select(asc => asc.SchoolId);
            
            query = _context.Classes
                .Where(c => userSchoolIdsQuery.Contains(c.SchoolId));
        }

        return await query
            .OrderBy(c => c.Name)
            .ProjectTo<ClassDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}