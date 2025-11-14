using Educar.Backend.Application.Commands.AnswerTypes;
using Educar.Backend.Application.Common.Extensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Common;
using Educar.Backend.Application.Commands.QuestStep.CreateFullQuestStep;

namespace Educar.Backend.Application.Commands.QuestStep.BulkCreateFullQuestStep;

public class FullQuestStepDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public QuestStepNpcType NpcType { get; set; }
    public QuestStepNpcBehaviour NpcBehaviour { get; set; }
    public QuestStepType Type { get; set; }
    public IList<CreateFullQuestStepContentDto> Contents { get; set; } = new List<CreateFullQuestStepContentDto>();
    public IList<Guid> NpcIds { get; set; } = new List<Guid>();
    public IList<Guid> MediaIds { get; set; } = new List<Guid>();
    public IList<Guid> ItemIds { get; set; } = new List<Guid>();
}

public record BulkCreateFullQuestStepCommand(
    Guid QuestId,
    IList<FullQuestStepDto> Steps) : IRequest<IEnumerable<IdResponseDto>>;

public class BulkCreateFullQuestStepCommandHandler : IRequestHandler<BulkCreateFullQuestStepCommand, IEnumerable<IdResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public BulkCreateFullQuestStepCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<IdResponseDto>> Handle(BulkCreateFullQuestStepCommand request, CancellationToken cancellationToken)
    {
        var quest = await _context.Quests.FirstOrDefaultAsync(e => e.Id == request.QuestId, cancellationToken);
        Guard.Against.NotFound(request.QuestId, quest, nameof(Quest));

        var createdStepIds = new List<IdResponseDto>();

        foreach (var stepDto in request.Steps)
        {
            var npcEntities = await GetNpcsByIdsAsync(stepDto.NpcIds, cancellationToken);
            var mediaEntities = await GetMediasByIdsAsync(stepDto.MediaIds, cancellationToken);
            var itemEntities = await GetItemsByIdsAsync(stepDto.ItemIds, cancellationToken);

            var questStep = new Domain.Entities.QuestStep(
                stepDto.Name,
                stepDto.Description,
                stepDto.Order,
                stepDto.NpcType,
                stepDto.NpcBehaviour,
                stepDto.Type)
            {
                Quest = quest
            };

            foreach (var content in stepDto.Contents)
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
            
            createdStepIds.Add(new IdResponseDto(questStep.Id));
        }

        // Salva TUDO em uma única transação atômica
        await _context.SaveChangesAsync(cancellationToken);

        return createdStepIds;
    }

    // Cole os mesmos métodos privados do seu handler original aqui
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