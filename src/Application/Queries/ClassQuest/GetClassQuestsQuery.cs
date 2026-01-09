using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.ClassQuest;

public record GetClassQuestsQuery : IRequest<List<ClassQuestDto>>
{
    public Guid? ClassId { get; init; }
    public Guid? QuestId { get; init; }
}

public class GetClassQuestsQueryHandler : IRequestHandler<GetClassQuestsQuery, List<ClassQuestDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetClassQuestsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ClassQuestDto>> Handle(GetClassQuestsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ClassQuests
            .Include(cq => cq.Class)
            .Include(cq => cq.Quest)
            .AsQueryable();

        // Filtrar por ClassId se fornecido
        if (request.ClassId.HasValue)
        {
            query = query.Where(cq => cq.ClassId == request.ClassId.Value);
        }

        // Filtrar por QuestId se fornecido
        if (request.QuestId.HasValue)
        {
            query = query.Where(cq => cq.QuestId == request.QuestId.Value);
        }

        var entities = await query.ToListAsync(cancellationToken);

        return _mapper.Map<List<ClassQuestDto>>(entities);
    }
}
