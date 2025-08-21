using AutoMapper.QueryableExtensions;
using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.Class;


public record GetClassesBySchoolsQuery : IRequest<List<ClassDto>>
{
    public List<Guid> SchoolIds { get; init; } = new();
}


public class GetClassesBySchoolsQueryHandler : IRequestHandler<GetClassesBySchoolsQuery, List<ClassDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetClassesBySchoolsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ClassDto>> Handle(GetClassesBySchoolsQuery request, CancellationToken cancellationToken)
    {
        if (request.SchoolIds == null || !request.SchoolIds.Any())
        {
            return new List<ClassDto>(); 
        }

        return await _context.Classes
            .Where(c => request.SchoolIds.Contains(c.SchoolId))
            .AsNoTracking()
            .ProjectTo<ClassDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}