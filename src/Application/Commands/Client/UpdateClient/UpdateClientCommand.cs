using Educar.Backend.Application.Commands.Contract.CreateContract;
using Educar.Backend.Application.Interfaces;

namespace Educar.Backend.Application.Commands.Client.UpdateClient;

public record UpdateClientCommand(string Name, string Description) : IRequest<Unit>
{
    public Guid Id { get; set; }
}

public class UpdateClientCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateClientCommand, Unit>
{
    public async Task<Unit> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Clients
            .Where(a => a.Id == request.Id)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.Name = request.Name;
        entity.Description = request.Description;

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}