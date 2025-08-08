using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Npc.DeleteNpc;

public record DeleteNpcCommand(Guid Id) : IRequest<Unit>;

public class DeleteGameCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteNpcCommand, Unit>
{
    public async Task<Unit> Handle(DeleteNpcCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Npcs
            .Include(gs => gs.NpcItems)
            .Include(gs => gs.GameNpcs)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.NpcItems.Clear();
        entity.GameNpcs.Clear();
        context.Npcs.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}