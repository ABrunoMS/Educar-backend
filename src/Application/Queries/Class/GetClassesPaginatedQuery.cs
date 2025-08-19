using AutoMapper.QueryableExtensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

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

    public GetClassesPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ClassDto>> Handle(GetClassesPaginatedQuery request, CancellationToken cancellationToken)
    {
        return await _context.Classes
            .OrderBy(x => x.Name) 
            .ProjectTo<ClassDto>(_mapper.ConfigurationProvider) 
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}