/*using Educar.Backend.Application.Commands.Game.DeleteGame;
using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Map.DeleteMap;

public record DeleteMapCommand(Guid Id) : IRequest<Unit>;

public class DeleteGameCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteMapCommand, Unit>
{
    public async Task<Unit> Handle(DeleteMapCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Maps
            .Include(gp => gp.SpawnPoints)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.SpawnPoints.Clear();
        context.Maps.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}*/