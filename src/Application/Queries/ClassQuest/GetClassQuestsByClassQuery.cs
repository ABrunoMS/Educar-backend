using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.ClassQuest;

public record GetClassQuestsByClassQuery : IRequest<List<ClassQuestDto>>
{
    public Guid ClassId { get; init; }
}

public class GetClassQuestsByClassQueryHandler : IRequestHandler<GetClassQuestsByClassQuery, List<ClassQuestDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetClassQuestsByClassQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ClassQuestDto>> Handle(GetClassQuestsByClassQuery request, CancellationToken cancellationToken)
    {
        var entities = await _context.ClassQuests
            .Include(cq => cq.Class)
            .Include(cq => cq.Quest)
            .Where(cq => cq.ClassId == request.ClassId)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<ClassQuestDto>>(entities);
    }
}
