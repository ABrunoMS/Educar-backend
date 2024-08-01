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
    public IList<Guid> DialogueIds { get; set; } = new List<Guid>();
}

public class UpdateNpcCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateNpcCommand, Unit>
{
    public async Task<Unit> Handle(UpdateNpcCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Npcs
            .Include(n => n.NpcItems)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        UpdateNpcProperties(entity, request);

        await UpdateNpcItems(context, entity, request.ItemIds, cancellationToken);

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

    private async Task UpdateNpcItems(IApplicationDbContext context, Domain.Entities.Npc entity,
        IList<Guid> itemIds, CancellationToken cancellationToken)
    {
        var itemEntities = await context.Items
            .Where(i => itemIds.Contains(i.Id))
            .ToListAsync(cancellationToken);

        var missingItemIds = itemIds.Except(itemEntities.Select(i => i.Id)).ToList();
        if (missingItemIds.Count != 0) throw new NotFoundException(nameof(Item), string.Join(", ", missingItemIds));

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

        foreach (var npcItem in itemsToRemove.Select(itemId =>
                     allNpcItems.First(ni => ni.ItemId == itemId)))
        {
            npcItem.IsDeleted = true;
            npcItem.DeletedAt = DateTimeOffset.UtcNow;
        }
    }
}