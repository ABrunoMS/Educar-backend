using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Proficiency.DeleteProficiency;

public record DeleteProficiencyCommand(Guid Id) : IRequest<Unit>;

public class DeleteProficiencyCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteProficiencyCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProficiencyCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Proficiencies
            .Include(gs => gs.ProficiencyGroupProficiencies)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.ProficiencyGroupProficiencies.Clear();
        context.Proficiencies.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}