using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.SpawnPoint.CreateSpawnPoint;

public record CreateSpawnPointCommand(
    string Name,
    string Reference,
    decimal X,
    decimal Y,
    decimal Z,
    Guid MapId)
    : IRequest<IdResponseDto>
{
    public string Name { get; set; } = Name;
    public string Reference { get; set; } = Reference;
    public decimal X { get; set; } = X;
    public decimal Y { get; set; } = Y;
    public decimal Z { get; set; } = Z;
    public Guid MapId { get; set; } = MapId;
}

public class CreateSpawnPointCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateSpawnPointCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateSpawnPointCommand request, CancellationToken cancellationToken)
    {
        // Ensure that the map exists before creating the spawn point
        var map = await context.Maps.FindAsync(new object[] { request.MapId }, cancellationToken: cancellationToken);
        if (map == null) throw new NotFoundException(nameof(Map), request.MapId.ToString());

        var entity = new Domain.Entities.SpawnPoint(request.Name, request.Reference, request.X, request.Y, request.Z)
        {
            MapId = request.MapId,
            Map = map
        };

        context.SpawnPoints.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}