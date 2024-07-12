using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Game.UpdateGame;

public record UpdateGameCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Lore { get; set; }
    public string? Purpose { get; set; }
}

public class UpdateGameCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateGameCommand, Unit>
{
    public async Task<Unit> Handle(UpdateGameCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Games
            .Where(a => a.Id == request.Id)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        if (request.Name != null) entity.Name = request.Name;
        if (request.Description != null) entity.Description = request.Description;
        if (request.Lore != null) entity.Lore = request.Lore;
        if (request.Purpose != null) entity.Purpose = request.Purpose;

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}