using Educar.Backend.Application.Commands.Contract.DeleteContract;
using Educar.Backend.Application.Interfaces;
using Educar.Backend.Domain.Events;

namespace Educar.Backend.Application.Commands.Client;

public record DeleteClientCommand(Guid Id) : IRequest<Unit>;

public class DeleteClientCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteClientCommand, Unit>
{
    public async Task<Unit> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Clients
            .Include(c => c.Contract)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        var contract = entity.Contract;
        if (contract != null)
        {
            entity.AddDomainEvent(new ClientDeletedEvent(entity));
        }

        context.Clients.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}