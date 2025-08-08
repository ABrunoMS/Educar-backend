using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.QuestStep.DeleteQuestStep;

public record DeleteQuestStepCommand(Guid Id) : IRequest<Unit>;

public class DeleteQuestStepCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteQuestStepCommand, Unit>
{
    public async Task<Unit> Handle(DeleteQuestStepCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.QuestSteps
            .Include(gs => gs.QuestStepMedias)
            .Include(gs => gs.QuestStepItems)
            .Include(gs => gs.QuestStepNpcs)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.QuestStepItems.Clear();
        entity.QuestStepMedias.Clear();
        entity.QuestStepNpcs.Clear();
        context.QuestSteps.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}