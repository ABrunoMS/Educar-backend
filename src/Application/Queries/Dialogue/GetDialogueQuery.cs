using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.Dialogue;

public class GetDialogueQuery : IRequest<DialogueDto>
{
    public Guid Id { get; init; }
}

public class GetDialogueQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetDialogueQuery, DialogueDto>
{
    public async Task<DialogueDto> Handle(GetDialogueQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.Dialogues
            .ProjectTo<DialogueDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.Dialogue), request.Id.ToString());

        return mapper.Map<DialogueDto>(entity);
    }
}