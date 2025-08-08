using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.Quest;

public record GetQuestQuery : IRequest<QuestDto>
{
    public Guid Id { get; init; }
}

public class GetQuestQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetQuestQuery, QuestDto>
{
    public async Task<QuestDto> Handle(GetQuestQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.Quests
            .Include(g => g.QuestProficiencies)
            .ThenInclude(gs => gs.Proficiency)
            .Include(g => g.QuestSteps)
            .ProjectTo<QuestDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, entity, nameof(Domain.Entities.Quest));

        //order queststeps by order
        entity.QuestSteps = entity.QuestSteps.OrderBy(qs => qs.Order).ToList();
        
        return mapper.Map<QuestDto>(entity);
    }
}