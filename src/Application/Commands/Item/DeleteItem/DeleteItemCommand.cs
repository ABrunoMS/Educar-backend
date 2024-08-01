using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Item.DeleteItem;

public record DeleteItemCommand(Guid Id) : IRequest<Unit>;

public class DeleteClientCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteItemCommand, Unit>
{
    public async Task<Unit> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Items
            .Include(i => i.NpcItems)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.NpcItems.Clear();
        context.Items.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}