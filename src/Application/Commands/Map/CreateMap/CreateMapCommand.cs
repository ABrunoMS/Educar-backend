using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Map.CreateMap;

public record CreateMapCommand(
    string Name,
    string Description,
    MapType Type,
    string Reference2D,
    string Reference3D,
    Guid GameId)
    : IRequest<IdResponseDto>
{
    public string Name { get; set; } = Name;
    public string Description { get; set; } = Description;
    public MapType Type { get; set; } = Type;
    public string Reference2D { get; set; } = Reference2D;
    public string Reference3D { get; set; } = Reference3D;
    public Guid GameId { get; set; } = GameId;
}

public class CreateMapCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateMapCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateMapCommand request, CancellationToken cancellationToken)
    {
        // Ensure that the game exists before creating the map
        var game = await context.Games.FindAsync(new object[] { request.GameId }, cancellationToken: cancellationToken);
        if (game == null) throw new NotFoundException(nameof(Game), request.GameId.ToString());

        var entity = new Domain.Entities.Map(request.Name, request.Description, request.Type, request.Reference2D,
            request.Reference3D)
        {
            Game = game
        };

        context.Maps.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}