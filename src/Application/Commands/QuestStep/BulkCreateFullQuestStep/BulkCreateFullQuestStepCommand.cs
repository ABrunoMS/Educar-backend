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
        Console.WriteLine($"[DEBUG BULK] Criando etapas para Quest ID: {request.QuestId}");
        Console.WriteLine($"[DEBUG BULK] Total de etapas a criar: {request.Steps?.Count ?? 0}");
        
        var quest = await _context.Quests.FirstOrDefaultAsync(e => e.Id == request.QuestId, cancellationToken);
        Guard.Against.NotFound(request.QuestId, quest, nameof(Quest));

        var createdStepIds = new List<IdResponseDto>();

        if (request.Steps != null)
        {
            foreach (var stepDto in request.Steps)
            {
                Console.WriteLine($"[DEBUG BULK] Criando etapa: {stepDto.Name}, Order: {stepDto.Order}, Contents: {stepDto.Contents?.Count ?? 0}");
            
            var npcEntities = await GetNpcsByIdsAsync(stepDto.NpcIds ?? new List<Guid>(), cancellationToken);
            var mediaEntities = await GetMediasByIdsAsync(stepDto.MediaIds ?? new List<Guid>(), cancellationToken);
            var itemEntities = await GetItemsByIdsAsync(stepDto.ItemIds ?? new List<Guid>(), cancellationToken);

            var questStep = new Domain.Entities.QuestStep(
                stepDto.Name,
                stepDto.Description,
                stepDto.Order,
                stepDto.NpcType,
                stepDto.NpcBehaviour,
                stepDto.Type)
            {
                Quest = quest,
                QuestId = quest.Id // Garantir que o QuestId está definido
            };

            Console.WriteLine($"[DEBUG BULK] QuestStep criado com QuestId: {questStep.QuestId}");

            if (stepDto.Contents != null)
            {
                foreach (var content in stepDto.Contents)
                {
                    if (content.ExpectedAnswers == null) continue; // Pula se não houver respostas esperadas
                    
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
            Console.WriteLine($"[DEBUG BULK] Etapa {questStep.Name} adicionada ao contexto com ID: {questStep.Id}");
            }
        }

        // Salva TUDO em uma única transação atômica
        Console.WriteLine($"[DEBUG BULK] Salvando {createdStepIds.Count} etapas no banco...");
        await _context.SaveChangesAsync(cancellationToken);
        Console.WriteLine($"[DEBUG BULK] Salvo com sucesso!");

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