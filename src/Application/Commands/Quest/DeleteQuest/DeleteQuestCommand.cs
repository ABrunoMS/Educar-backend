using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Quest.DeleteQuest;

public record DeleteQuestCommand(Guid Id) : IRequest<Unit>;

public class DeleteQuestCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteQuestCommand, Unit>
{
    public async Task<Unit> Handle(DeleteQuestCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Quests
            .Include(gs => gs.QuestProficiencies)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.QuestProficiencies.Clear();
        context.Quests.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}