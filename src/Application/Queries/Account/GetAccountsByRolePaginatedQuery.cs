using AutoMapper;
using AutoMapper.QueryableExtensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Domain.Enums;
using MediatR;

namespace Educar.Backend.Application.Queries.Account;

public class GetAccountsByRolePaginatedQuery : IRequest<PaginatedList<CleanAccountDto>>
{
    public UserRole Role { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 100; // Um numero alto para o dropdown
}



public class GetAccountsByRolePaginatedQueryHandler : IRequestHandler<GetAccountsByRolePaginatedQuery, PaginatedList<CleanAccountDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAccountsByRolePaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<CleanAccountDto>> Handle(GetAccountsByRolePaginatedQuery request, CancellationToken cancellationToken)
    {
        return await _context.Accounts
            .Where(x => x.Role == request.Role) // <--- O FILTRO MÁGICO
            .OrderBy(x => x.Name)
            .ProjectTo<CleanAccountDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
