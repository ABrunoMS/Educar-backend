using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Dialogue.UpdateDialogue;

public record UpdateDialogueCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
    public int? Order { get; set; }
    public Guid NpcId { get; set; }
}

public class UpdateDialogueCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateDialogueCommand, Unit>
{
    public async Task<Unit> Handle(UpdateDialogueCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Dialogues
            .Where(a => a.Id == request.Id)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        var npc = await context.Npcs.FindAsync([request.NpcId, cancellationToken],
            cancellationToken: cancellationToken);
        Guard.Against.NotFound(request.NpcId, npc);

        entity.Text = request.Text ?? entity.Text;
        entity.Order = request.Order ?? entity.Order;
        entity.Npc = npc;

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}