using Educar.Backend.Application.Common.Interfaces;
using NotFoundException = Educar.Backend.Application.Common.Exceptions.NotFoundException;

namespace Educar.Backend.Application.Queries.ClassQuest;

public record GetClassQuestQuery : IRequest<ClassQuestDto>
{
    public Guid ClassId { get; init; }
    public Guid QuestId { get; init; }
}

public class GetClassQuestQueryHandler : IRequestHandler<GetClassQuestQuery, ClassQuestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetClassQuestQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ClassQuestDto> Handle(GetClassQuestQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.ClassQuests
            .Include(cq => cq.Class)
            .Include(cq => cq.Quest)
            .FirstOrDefaultAsync(cq => cq.ClassId == request.ClassId && cq.QuestId == request.QuestId, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(Domain.Entities.ClassQuest), $"ClassId: {request.ClassId}, QuestId: {request.QuestId}");

        return _mapper.Map<ClassQuestDto>(entity);
    }
}
