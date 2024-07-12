using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Game.CreateGame;

public record CreateGameCommand(string Name, string Description, string Lore, string Purpose)
    : IRequest<CreatedResponseDto>
{
    public string Name { get; set; } = Name;
    public string Description { get; set; } = Description;
    public string Lore { get; set; } = Lore;
    public string Purpose { get; set; } = Purpose;
}

public class CreateGameCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateGameCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Game(request.Name, request.Description, request.Lore, request.Purpose);

        context.Games.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}