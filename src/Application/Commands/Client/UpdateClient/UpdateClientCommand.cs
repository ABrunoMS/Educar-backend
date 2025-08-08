using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Client.UpdateClient;

public record UpdateClientCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class UpdateClientCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateClientCommand, Unit>
{
    public async Task<Unit> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Clients
            .Where(a => a.Id == request.Id)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        if (request.Name != null) entity.Name = request.Name;
        entity.Description = request.Description;

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}