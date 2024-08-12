using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.QuestStepContent;

public class GetQuestStepContentQuery : IRequest<QuestStepContentDto>
{
    public Guid Id { get; init; }
}

public class GetQuestStepContentQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetQuestStepContentQuery, QuestStepContentDto>
{
    public async Task<QuestStepContentDto> Handle(GetQuestStepContentQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.QuestStepContents
            .ProjectTo<QuestStepContentDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(Domain.Entities.QuestStepContent), request.Id.ToString());

        var QuestStepContentDto = mapper.Map<QuestStepContentDto>(entity);

        return QuestStepContentDto;
    }
}