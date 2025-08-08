using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Dialogue.DeleteDialogue;

public record DeleteDialogueCommand(Guid Id) : IRequest<Unit>;

public class DeleteContractCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteDialogueCommand, Unit>
{
    public async Task<Unit> Handle(DeleteDialogueCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Dialogues.FindAsync([request.Id], cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        context.Dialogues.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}