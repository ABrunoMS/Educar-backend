using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.ProficiencyGroup.DeleteProficiencyGroup;

public record DeleteProficiencyGroupCommand(Guid Id) : IRequest<Unit>;

public class DeleteProficiencyGroupCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteProficiencyGroupCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProficiencyGroupCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.ProficiencyGroups
            .Include(gs => gs.ProficiencyGroupProficiencies)
            .Include(gs => gs.GameProficiencyGroups)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.ProficiencyGroupProficiencies.Clear();
        entity.GameProficiencyGroups.Clear();
        context.ProficiencyGroups.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}