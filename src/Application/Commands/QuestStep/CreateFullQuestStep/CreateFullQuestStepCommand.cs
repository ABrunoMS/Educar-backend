using Educar.Backend.Application.Commands.AnswerTypes;
using Educar.Backend.Application.Common.Extensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Common;

namespace Educar.Backend.Application.Commands.QuestStep.CreateFullQuestStep;

public class CreateFullQuestStepContentDto
{
    public QuestStepContentType QuestStepContentType { get; set; }
    public QuestionType QuestionType { get; set; }
    public string Description { get; set; } = string.Empty;
    public IAnswer ExpectedAnswers { get; set; } = null!;
    public decimal Weight { get; set; }
}

public record CreateFullQuestStepCommand(
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
    public IList<CreateFullQuestStepContentDto> Contents { get; set; } = new List<CreateFullQuestStepContentDto>();
    public IList<Guid> NpcIds { get; set; } = new List<Guid>();
    public IList<Guid> MediaIds { get; set; } = new List<Guid>();
    public IList<Guid> ItemIds { get; set; } = new List<Guid>();
    public Guid QuestId { get; set; } = QuestId;
}

public class CreateFullQuestStepCommandHandler : IRequestHandler<CreateFullQuestStepCommand, IdResponseDto>
{
    private readonly IApplicationDbContext _context;

    public CreateFullQuestStepCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IdResponseDto> Handle(CreateFullQuestStepCommand request, CancellationToken cancellationToken)
    {
        var npcEntities = await GetNpcsByIdsAsync(request.NpcIds, cancellationToken);
        var mediaEntities = await GetMediasByIdsAsync(request.MediaIds, cancellationToken);
        var itemEntities = await GetItemsByIdsAsync(request.ItemIds, cancellationToken);

        var quest = await _context.Quests.FirstOrDefaultAsync(e => e.Id == request.QuestId, cancellationToken);
        Guard.Against.NotFound(request.QuestId, quest, nameof(Quest));

        var questStep = new Domain.Entities.QuestStep(
            request.Name,
            request.Description,
            request.Order,
            request.NpcType,
            request.NpcBehaviour,
            request.Type)
        {
            Quest = quest
        };

        foreach (var content in request.Contents)
        {
            var questStepContent = new Domain.Entities.QuestStepContent(
                content.QuestStepContentType,
                content.QuestionType,
                content.Description,
                content.ExpectedAnswers.ToJsonObject(),
                content.Weight)
            {
                QuestStep = questStep
            };

            questStep.Contents.Add(questStepContent);
        }

        foreach (var npcEntity in npcEntities)
        {
            questStep.QuestStepNpcs.Add(new QuestStepNpc { QuestStep = questStep, Npc = npcEntity });
        }

        foreach (var mediaEntity in mediaEntities)
        {
            questStep.QuestStepMedias.Add(new QuestStepMedia { QuestStep = questStep, Media = mediaEntity });
        }

        foreach (var itemEntity in itemEntities)
        {
            questStep.QuestStepItems.Add(new QuestStepItem { QuestStep = questStep, Item = itemEntity });
        }

        _context.QuestSteps.Add(questStep);

        await _context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(questStep.Id);
    }

    private async Task<List<Domain.Entities.Npc>> GetNpcsByIdsAsync(IList<Guid> ids, CancellationToken cancellationToken)
    {
        return await GetEntitiesByIdsAsync(_context.Npcs, ids, cancellationToken);
    }

    private async Task<List<Domain.Entities.Media>> GetMediasByIdsAsync(IList<Guid> ids, CancellationToken cancellationToken)
    {
        return await GetEntitiesByIdsAsync(_context.Medias, ids, cancellationToken);
    }

    private async Task<List<Domain.Entities.Item>> GetItemsByIdsAsync(IList<Guid> ids, CancellationToken cancellationToken)
    {
        return await GetEntitiesByIdsAsync(_context.Items, ids, cancellationToken);
    }

    private async Task<List<TEntity>> GetEntitiesByIdsAsync<TEntity>(
        DbSet<TEntity> dbSet,
        IList<Guid> ids,
        CancellationToken cancellationToken) where TEntity : BaseEntity
    {
        if (ids == null || !ids.Any())
            return new List<TEntity>();

        var entities = await dbSet
            .Where(e => ids.Contains(e.Id))
            .ToListAsync(cancellationToken);

        var missingIds = ids.Except(entities.Select(e => e.Id)).ToList();
        if (missingIds.Any())
            throw new NotFoundException(typeof(TEntity).Name, string.Join(", ", missingIds));

        return entities;
    }
}