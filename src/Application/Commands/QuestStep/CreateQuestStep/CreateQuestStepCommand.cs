using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Common;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.QuestStep.CreateQuestStep;

public record CreateQuestStepCommand(
    string Name,
    string Description,
    int Order,
    QuestStepNpcType NpcType,
    QuestStepNpcBehaviour NpcBehaviour,
    QuestStepType QuestStepType,
    Guid QuestId) : IRequest<IdResponseDto>
{
    public string Name { get; set; } = Name;
    public string Description { get; set; } = Description;
    public int Order { get; set; } = Order;
    public QuestStepNpcType NpcType { get; set; } = NpcType;
    public QuestStepNpcBehaviour NpcBehaviour { get; set; } = NpcBehaviour;
    public QuestStepType Type { get; set; } = QuestStepType;
    public IList<Guid> ContentIds { get; set; } = new List<Guid>();
    public IList<Guid> NpcIds { get; set; } = new List<Guid>();
    public IList<Guid> MediaIds { get; set; } = new List<Guid>();
    public IList<Guid> ItemIds { get; set; } = new List<Guid>();
    public Guid QuestId { get; set; } = QuestId;
}

public class CreateQuestStepCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateQuestStepCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateQuestStepCommand request, CancellationToken cancellationToken)
    {
        // Retrieve related entities
        var contentEntities = await GetQuestStepContentsByIdsAsync(request.ContentIds, cancellationToken);
        var npcEntities = await GetNpcsByIdsAsync(request.NpcIds, cancellationToken);
        var mediaEntities = await GetMediasByIdsAsync(request.MediaIds, cancellationToken);
        var itemEntities = await GetItemsByIdsAsync(request.ItemIds, cancellationToken);

        var quest = await context.Quests.FirstOrDefaultAsync(e => e.Id == request.QuestId, cancellationToken);
        Guard.Against.NotFound(request.QuestId, quest, nameof(Domain.Entities.Quest));

        // Create the QuestStep entity
        var entity = new Domain.Entities.QuestStep(
            request.Name,
            request.Description,
            request.Order,
            request.NpcType,
            request.NpcBehaviour,
            request.QuestStepType)
        {
            Contents = contentEntities.ToList(),
            Quest = quest
        };

        // Associate NPCs with QuestStep
        foreach (var npcEntity in npcEntities)
        {
            entity.QuestStepNpcs.Add(new QuestStepNpc { QuestStep = entity, Npc = npcEntity });
        }

        // Associate media with QuestStep
        foreach (var mediaEntity in mediaEntities)
        {
            entity.QuestStepMedias.Add(new QuestStepMedia { QuestStep = entity, Media = mediaEntity });
        }

        // Associate items with QuestStep
        foreach (var itemEntity in itemEntities)
        {
            entity.QuestStepItems.Add(new QuestStepItem { QuestStep = entity, Item = itemEntity });
        }

        // Add the QuestStep entity to the context
        context.QuestSteps.Add(entity);

        // Save changes
        await context.SaveChangesAsync(cancellationToken);

        // Return the created response with the new QuestStep ID
        return new IdResponseDto(entity.Id);
    }

    private async Task<List<Domain.Entities.QuestStepContent>> GetQuestStepContentsByIdsAsync(IList<Guid> ids,
        CancellationToken cancellationToken)
    {
        return await GetEntitiesByIdsAsync(context.QuestStepContents, ids, cancellationToken);
    }

    private async Task<List<Domain.Entities.Npc>> GetNpcsByIdsAsync(IList<Guid> ids,
        CancellationToken cancellationToken)
    {
        return await GetEntitiesByIdsAsync(context.Npcs, ids, cancellationToken);
    }

    private async Task<List<Domain.Entities.Media>> GetMediasByIdsAsync(IList<Guid> ids,
        CancellationToken cancellationToken)
    {
        return await GetEntitiesByIdsAsync(context.Medias, ids, cancellationToken);
    }

    private async Task<List<Domain.Entities.Item>> GetItemsByIdsAsync(IList<Guid> ids,
        CancellationToken cancellationToken)
    {
        return await GetEntitiesByIdsAsync(context.Items, ids, cancellationToken);
    }

    private async Task<List<TEntity>> GetEntitiesByIdsAsync<TEntity>(
        DbSet<TEntity> dbSet,
        IList<Guid> ids,
        CancellationToken cancellationToken)
        where TEntity : BaseEntity
    {
        if (ids == null || ids.Count == 0)
        {
            return [];
        }

        var entities = await dbSet
            .Where(e => ids.Contains(e.Id))
            .ToListAsync(cancellationToken);

        var entityIds = entities.Select(e => e.Id).ToList();
        var missingIds = ids.Except(entityIds).ToList();

        if (missingIds.Count > 0)
        {
            throw new NotFoundException(typeof(TEntity).Name, string.Join(", ", missingIds));
        }

        return entities;
    }
}