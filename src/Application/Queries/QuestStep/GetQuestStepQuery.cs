using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.QuestStep;

public class GetQuestStepQuery : IRequest<QuestStepDto>
{
    public Guid Id { get; init; }
}

public class GetQuestStepQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetQuestStepQuery, QuestStepDto>
{
    public async Task<QuestStepDto> Handle(GetQuestStepQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.QuestSteps
            .Include(g => g.QuestStepItems)
            .ThenInclude(gs => gs.Item)
            .Include(g => g.QuestStepMedias)
            .ThenInclude(gs => gs.Media)
            .Include(g => g.QuestStepNpcs)
            .ThenInclude(gs => gs.Npc)
            .Include(qs => qs.Contents)
            .ProjectTo<QuestStepDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.QuestStep), request.Id.ToString());

        var QuestStepDto = mapper.Map<QuestStepDto>(entity);

        return QuestStepDto;
    }
}