using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Npc.CreateNpc;

public record CreateNpcCommand(string Name, string Lore, NpcType NpcType, decimal GoldDropRate, decimal GoldAmount)
    : IRequest<CreatedResponseDto>
{
    public string Name { get; set; } = Name;
    public string Lore { get; set; } = Lore;
    public NpcType NpcType { get; set; } = NpcType;
    public decimal GoldDropRate { get; set; } = GoldDropRate;
    public decimal GoldAmount { get; set; } = GoldAmount;
    public IList<Guid> ItemIds { get; set; } = new List<Guid>();
    public IList<Guid> GameIds { get; set; } = new List<Guid>();
}

public class CreateNpcCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateNpcCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(CreateNpcCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the items
        var itemEntities = new List<Domain.Entities.Item>();
        if (request.ItemIds != null && request.ItemIds.Count != 0)
        {
            itemEntities = await context.Items
                .Where(i => request.ItemIds.Contains(i.Id))
                .ToListAsync(cancellationToken);

            var missingItemIds = request.ItemIds.Except(itemEntities.Select(i => i.Id)).ToList();
            if (missingItemIds.Count != 0)
            {
                throw new NotFoundException(nameof(Domain.Entities.Item), string.Join(", ", missingItemIds));
            }
        }

        // Retrieve the games
        var gameEntities = new List<Domain.Entities.Game>();
        if (request.GameIds != null && request.GameIds.Count != 0)
        {
            gameEntities = await context.Games
                .Where(g => request.GameIds.Contains(g.Id))
                .ToListAsync(cancellationToken);

            var missingGameIds = request.GameIds.Except(gameEntities.Select(g => g.Id)).ToList();
            if (missingGameIds.Count != 0)
            {
                throw new NotFoundException(nameof(Domain.Entities.Game), string.Join(", ", missingGameIds));
            }
        }

        var entity = new Domain.Entities.Npc(request.Name, request.Lore, request.NpcType, request.GoldDropRate,
            request.GoldAmount);

        // Associate items with NPC
        foreach (var itemEntity in itemEntities)
        {
            entity.NpcItems.Add(new NpcItem { Npc = entity, Item = itemEntity });
        }

        // Associate games with NPC
        foreach (var gameEntity in gameEntities)
        {
            entity.GameNpcs.Add(new GameNpc { Npc = entity, Game = gameEntity });
        }

        context.Npcs.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}