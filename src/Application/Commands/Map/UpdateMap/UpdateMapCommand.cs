/*using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Map.UpdateMap;

public record UpdateMapCommand(
    Guid Id,
    string Name,
    string Description,
    MapType Type,
    string Reference2D,
    string Reference3D)
    : IRequest<Unit>
{
    public Guid Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    public string Description { get; set; } = Description;
    public MapType Type { get; set; } = Type;
    public string Reference2D { get; set; } = Reference2D;
    public string Reference3D { get; set; } = Reference3D;
}

public class UpdateMapCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateMapCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMapCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the map from the database
        var entity = await context.Maps.FindAsync(new object[] { request.Id }, cancellationToken: cancellationToken);
        if (entity == null) throw new NotFoundException(nameof(Map), request.Id.ToString());

        // Update the map properties
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Type = request.Type;
        entity.Reference2D = request.Reference2D;
        entity.Reference3D = request.Reference3D;

        // Save the updated map
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}*/