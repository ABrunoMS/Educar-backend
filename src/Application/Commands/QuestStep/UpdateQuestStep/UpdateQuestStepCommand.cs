using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Infrastructure.Data;

namespace Educar.Backend.Application.Commands.QuestStep.UpdateQuestStep;

public record UpdateQuestStepCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Order { get; set; }
    public QuestStepNpcType? NpcType { get; set; }
    public QuestStepNpcBehaviour? NpcBehaviour { get; set; }
    public QuestStepType? Type { get; set; }
    public bool? IsActive { get; set; }
    public IList<Guid> ContentIds { get; set; } = new List<Guid>();
    public IList<Guid> NpcIds { get; set; } = new List<Guid>();
    public IList<Guid> MediaIds { get; set; } = new List<Guid>();
    public IList<Guid> ItemIds { get; set; } = new List<Guid>();
}

public class UpdateQuestStepCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateQuestStepCommand, Unit>
{
    public async Task<Unit> Handle(UpdateQuestStepCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.QuestSteps
            .Include(qs => qs.Contents)
            .Include(qs => qs.QuestStepNpcs)
            .Include(qs => qs.QuestStepMedias)
            .Include(qs => qs.QuestStepItems)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        UpdateQuestStepProperties(entity, request);

        await UpdateQuestStepContents(context, entity, request.ContentIds, cancellationToken);
        await UpdateQuestStepNpcs(context, entity, request.NpcIds, cancellationToken);
        await UpdateQuestStepMedias(context, entity, request.MediaIds, cancellationToken);
        await UpdateQuestStepItems(context, entity, request.ItemIds, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private void UpdateQuestStepProperties(Domain.Entities.QuestStep entity, UpdateQuestStepCommand request)
    {
        if (request.Name != null) entity.Name = request.Name;
        if (request.Description != null) entity.Description = request.Description;
        if (request.Order.HasValue) entity.Order = request.Order.Value;
        if (request.NpcType.HasValue) entity.NpcType = request.NpcType.Value;
        if (request.NpcBehaviour.HasValue) entity.NpcBehaviour = request.NpcBehaviour.Value;
        if (request.Type.HasValue) entity.QuestStepType = request.Type.Value;
        if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
    }

    private async Task UpdateQuestStepContents(IApplicationDbContext context, Domain.Entities.QuestStep entity,
        IList<Guid> contentIds,
        CancellationToken cancellationToken)
    {
        var contentEntities = await context.QuestStepContents
            .Where(qsc => contentIds.Contains(qsc.Id))
            .ToListAsync(cancellationToken);

        var missingContentIds = contentIds.Except(contentEntities.Select(qsc => qsc.Id)).ToList();
        if (missingContentIds.Any())
            throw new NotFoundException(nameof(QuestStepContent), string.Join(", ", missingContentIds));

        var allQuestStepContents = await context.QuestStepContents
            .IgnoreQueryFilters()
            .Where(qsc => qsc.QuestStepId == entity.Id)
            .ToListAsync(cancellationToken);

        UpdateRelationships(entity.Contents, allQuestStepContents, contentEntities, contentIds, e => e.Id);
    }

    private async Task UpdateQuestStepNpcs(IApplicationDbContext context, Domain.Entities.QuestStep entity,
        IList<Guid> npcIds,
        CancellationToken cancellationToken)
    {
        var npcEntities = await context.QuestStepNpcs
            .Where(n => npcIds.Contains(n.NpcId))
            .ToListAsync(cancellationToken);

        var missingNpcIds = npcIds.Except(npcEntities.Select(n => n.NpcId)).ToList();
        if (missingNpcIds.Count != 0) throw new NotFoundException(nameof(Npc), string.Join(", ", missingNpcIds));

        var allQuestStepNpcs = await context.QuestStepNpcs
            .IgnoreQueryFilters()
            .Where(qsn => qsn.QuestStepId == entity.Id)
            .ToListAsync(cancellationToken);

        UpdateRelationships(entity.QuestStepNpcs, allQuestStepNpcs, npcEntities, npcIds, e => e.NpcId);
    }

    private async Task UpdateQuestStepMedias(IApplicationDbContext context, Domain.Entities.QuestStep entity,
        IList<Guid> mediaIds,
        CancellationToken cancellationToken)
    {
        var mediaEntities = await context.QuestStepMedias
            .Where(m => mediaIds.Contains(m.MediaId))
            .ToListAsync(cancellationToken);

        var missingMediaIds = mediaIds.Except(mediaEntities.Select(m => m.MediaId)).ToList();
        if (missingMediaIds.Count != 0) throw new NotFoundException(nameof(Media), string.Join(", ", missingMediaIds));

        var allQuestStepMedias = await context.QuestStepMedias
            .IgnoreQueryFilters()
            .Where(qsm => qsm.QuestStepId == entity.Id)
            .ToListAsync(cancellationToken);

        UpdateRelationships(entity.QuestStepMedias, allQuestStepMedias, mediaEntities, mediaIds, e => e.MediaId);
    }

    private async Task UpdateQuestStepItems(IApplicationDbContext context, Domain.Entities.QuestStep entity,
        IList<Guid> itemIds,
        CancellationToken cancellationToken)
    {
        var itemEntities = await context.QuestStepItems
            .Where(i => itemIds.Contains(i.ItemId))
            .ToListAsync(cancellationToken);

        var missingItemIds = itemIds.Except(itemEntities.Select(i => i.ItemId)).ToList();
        if (missingItemIds.Count != 0) throw new NotFoundException(nameof(Item), string.Join(", ", missingItemIds));

        var allQuestStepItems = await context.QuestStepItems
            .IgnoreQueryFilters()
            .Where(qsi => qsi.QuestStepId == entity.Id)
            .ToListAsync(cancellationToken);

        UpdateRelationships(entity.QuestStepItems, allQuestStepItems, itemEntities, itemIds, e => e.ItemId);
    }

    private void UpdateRelationships<T>(ICollection<T> existingEntities, List<T> allEntities, List<T> newEntities,
        IList<Guid> newEntityIds, Func<T, Guid> getId)
        where T : ISoftDelete
    {
        var currentEntityIds = allEntities.Where(e => !e.IsDeleted).Select(getId).ToList();
        var entitiesToAdd = newEntityIds.Except(currentEntityIds).ToList();
        var entitiesToRemove = currentEntityIds.Except(newEntityIds).ToList();

        foreach (var entityId in entitiesToAdd)
        {
            var existingEntity = allEntities.FirstOrDefault(e => getId(e) == entityId && e.IsDeleted);
            if (existingEntity != null)
            {
                existingEntity.IsDeleted = false;
                existingEntity.DeletedAt = null; // Reset DeletedAt
            }
            else
            {
                var newEntity = newEntities.First(e => getId(e) == entityId);
                existingEntities.Add(newEntity);
            }
        }

        foreach (var entity in entitiesToRemove.Select(entityId => allEntities.First(e => getId(e) == entityId)))
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTimeOffset.UtcNow; // Set DeletedAt
        }
    }
}