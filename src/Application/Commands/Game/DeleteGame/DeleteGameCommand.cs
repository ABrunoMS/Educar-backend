using Educar.Backend.Application.Common.Interfaces;
using ValidationException = Educar.Backend.Application.Common.Exceptions.ValidationException;

namespace Educar.Backend.Application.Commands.Game.DeleteGame;

public record DeleteGameCommand(Guid Id) : IRequest<Unit>;

public class DeleteGameCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteGameCommand, Unit>
{
    public async Task<Unit> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Games
            .Include(gs => gs.GameSubjects)
            .Include(gp => gp.GameProficiencyGroups)
            .Include(gp => gp.GameNpcs)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        var hasContracts = await context.Contracts.AnyAsync(c => c.GameId == request.Id, cancellationToken);
        if (hasContracts)
        {
            throw new Exception("This game has contracts and cannot be deleted.");
        }
        
        entity.GameSubjects.Clear();
        entity.GameProficiencyGroups.Clear();
        entity.GameNpcs.Clear();
        context.Games.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}