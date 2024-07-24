using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Class.DeleteClass;

public record DeleteClassCommand(Guid Id) : IRequest<Unit>;

public class DeleteClassCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteClassCommand, Unit>
{
    public async Task<Unit> Handle(DeleteClassCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Classes
            .Include(a => a.AccountClasses)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        context.Classes.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}