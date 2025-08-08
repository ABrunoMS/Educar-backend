using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Npc.UpdateNpc;

public record UpdateNpcCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Lore { get; set; }
    public NpcType? NpcType { get; set; }
    public decimal? GoldDropRate { get; set; }
    public decimal? GoldAmount { get; set; }
    public IList<Guid> ItemIds { get; set; } = new List<Guid>();
    public IList<Guid> GameIds { get; set; } = new List<Guid>();
}

public class UpdateNpcCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateNpcCommand, Unit>
{
    public async Task<Unit> Handle(UpdateNpcCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Npcs
            .Include(n => n.NpcItems)
            .Include(n => n.GameNpcs)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        UpdateNpcProperties(entity, request);

        await UpdateNpcItems(context, entity, request.ItemIds, cancellationToken);
        await UpdateNpcGames(context, entity, request.GameIds, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private void UpdateNpcProperties(Domain.Entities.Npc entity, UpdateNpcCommand request)
    {
        if (request.Name != null) entity.Name = request.Name;
        if (request.Lore != null) entity.Lore = request.Lore;
        if (request.NpcType.HasValue) entity.NpcType = request.NpcType.Value;
        if (request.GoldDropRate.HasValue) entity.GoldDropRate = request.GoldDropRate.Value;
        if (request.GoldAmount.HasValue) entity.GoldAmount = request.GoldAmount.Value;
    }

    private async Task UpdateNpcItems(IApplicationDbContext context, Domain.Entities.Npc entity, IList<Guid> itemIds,
        CancellationToken cancellationToken)
    {
        var itemEntities = await context.Items
            .Where(i => itemIds.Contains(i.Id))
            .ToListAsync(cancellationToken);

        var missingItemIds = itemIds.Except(itemEntities.Select(i => i.Id)).ToList();
        if (missingItemIds.Any()) throw new NotFoundException(nameof(Item), string.Join(", ", missingItemIds));

        var allNpcItems = await context.NpcItems
            .IgnoreQueryFilters()
            .Where(ni => ni.NpcId == entity.Id)
            .ToListAsync(cancellationToken);

        UpdateItemRelationships(entity, allNpcItems, itemEntities, itemIds);
    }

    private void UpdateItemRelationships(Domain.Entities.Npc entity, List<NpcItem> allNpcItems,
        List<Domain.Entities.Item> itemEntities, IList<Guid> itemIds)
    {
        var currentItemIds = allNpcItems.Where(ni => !ni.IsDeleted).Select(ni => ni.ItemId).ToList();
        var itemsToAdd = itemIds.Except(currentItemIds).ToList();
        var itemsToRemove = currentItemIds.Except(itemIds).ToList();

        foreach (var itemId in itemsToAdd)
        {
            var existingNpcItem = allNpcItems.FirstOrDefault(ni => ni.ItemId == itemId && ni.IsDeleted);
            if (existingNpcItem != null)
            {
                existingNpcItem.IsDeleted = false;
                existingNpcItem.DeletedAt = null;
            }
            else
            {
                var itemEntity = itemEntities.First(i => i.Id == itemId);
                entity.NpcItems.Add(new NpcItem { NpcId = entity.Id, ItemId = itemEntity.Id });
            }
        }

        foreach (var npcItem in itemsToRemove.Select(itemId => allNpcItems.First(ni => ni.ItemId == itemId)))
        {
            npcItem.IsDeleted = true;
            npcItem.DeletedAt = DateTimeOffset.UtcNow;
        }
    }

    private async Task UpdateNpcGames(IApplicationDbContext context, Domain.Entities.Npc entity, IList<Guid> gameIds,
        CancellationToken cancellationToken)
    {
        var gameEntities = await context.Games
            .Where(g => gameIds.Contains(g.Id))
            .ToListAsync(cancellationToken);

        var missingGameIds = gameIds.Except(gameEntities.Select(g => g.Id)).ToList();
        if (missingGameIds.Count != 0) throw new NotFoundException(nameof(Game), string.Join(", ", missingGameIds));

        var allNpcGames = await context.GameNpcs
            .IgnoreQueryFilters()
            .Where(gn => gn.NpcId == entity.Id)
            .ToListAsync(cancellationToken);

        UpdateGameRelationships(entity, allNpcGames, gameEntities, gameIds);
    }

    private void UpdateGameRelationships(Domain.Entities.Npc entity, List<GameNpc> allNpcGames,
        List<Domain.Entities.Game> gameEntities, IList<Guid> gameIds)
    {
        var currentGameIds = allNpcGames.Where(gn => !gn.IsDeleted).Select(gn => gn.GameId).ToList();
        var gamesToAdd = gameIds.Except(currentGameIds).ToList();
        var gamesToRemove = currentGameIds.Except(gameIds).ToList();

        foreach (var gameId in gamesToAdd)
        {
            var existingNpcGame = allNpcGames.FirstOrDefault(gn => gn.GameId == gameId && gn.IsDeleted);
            if (existingNpcGame != null)
            {
                existingNpcGame.IsDeleted = false;
                existingNpcGame.DeletedAt = null;
            }
            else
            {
                var gameEntity = gameEntities.First(g => g.Id == gameId);
                entity.GameNpcs.Add(new GameNpc { NpcId = entity.Id, GameId = gameEntity.Id });
            }
        }

        foreach (var npcGame in gamesToRemove.Select(gameId => allNpcGames.First(gn => gn.GameId == gameId)))
        {
            npcGame.IsDeleted = true;
            npcGame.DeletedAt = DateTimeOffset.UtcNow;
        }
    }
}