using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Events;

namespace Educar.Backend.Application.Commands.Client.DeleteClient;

public record DeleteClientCommand(Guid Id) : IRequest<Unit>;

public class DeleteClientCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteClientCommand, Unit>
{
    public async Task<Unit> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Clients
            //.Include(c => c.Contracts)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        // entity.AddDomainEvent(new ClientDeletedEvent(entity));

        context.Clients.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}