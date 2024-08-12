using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.QuestStepContent.DeleteQuestStepContent;

public record DeleteQuestStepContentCommand(Guid Id) : IRequest<Unit>;

public class DeleteQuestStepContentCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteQuestStepContentCommand, Unit>
{
    public async Task<Unit> Handle(DeleteQuestStepContentCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.QuestStepContents
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        context.QuestStepContents.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}