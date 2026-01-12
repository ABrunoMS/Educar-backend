using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.ClassQuest;

public record GetClassQuestsByQuestQuery : IRequest<List<ClassQuestDto>>
{
    public Guid QuestId { get; init; }
}

public class GetClassQuestsByQuestQueryHandler : IRequestHandler<GetClassQuestsByQuestQuery, List<ClassQuestDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetClassQuestsByQuestQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ClassQuestDto>> Handle(GetClassQuestsByQuestQuery request, CancellationToken cancellationToken)
    {
        var entities = await _context.ClassQuests
            .Include(cq => cq.Class)
            .Include(cq => cq.Quest)
            .Where(cq => cq.QuestId == request.QuestId)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<ClassQuestDto>>(entities);
    }
}
