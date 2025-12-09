using Educar.Backend.Application.Commands.AnswerTypes;
using Educar.Backend.Application.Common.Extensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Common;

namespace Educar.Backend.Application.Commands.Quest.CreateFullQuest;

public class CreateFullQuestStepContentDto
{
    public QuestStepContentType QuestStepContentType { get; set; }
    public QuestionType QuestionType { get; set; }
    public string Description { get; set; } = string.Empty;
    public IAnswer ExpectedAnswer { get; set; } = null!;
    public decimal Weight { get; set; }
}

public class CreateFullQuestStepDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public QuestStepNpcType NpcType { get; set; }
    public QuestStepNpcBehaviour NpcBehavior { get; set; }
    public QuestStepType QuestStepType { get; set; }
    public IList<CreateFullQuestStepContentDto> Contents { get; set; } = new List<CreateFullQuestStepContentDto>();
    public IList<Guid> NpcIds { get; set; } = new List<Guid>();
    public IList<Guid> MediaIds { get; set; } = new List<Guid>();
    public IList<Guid> ItemIds { get; set; } = new List<Guid>();
}

public record CreateFullQuestCommand(
    string Name,
    string Description,
    bool UsageTemplate,
    QuestType Type,
    int MaxPlayers,
    int TotalQuestSteps,
    CombatDifficulty CombatDifficulty) : IRequest<IdResponseDto>
{
    public string Name { get; set; } = Name;
    public string Description { get; set; } = Description;
    public bool UsageTemplate { get; set; } = UsageTemplate;
    public QuestType Type { get; set; } = Type;
    public int MaxPlayers { get; set; } = MaxPlayers;
    public int TotalQuestSteps { get; set; } = TotalQuestSteps;
    public CombatDifficulty CombatDifficulty { get; set; } = CombatDifficulty;
    public Guid? GameId { get; set; }
    public Guid? GradeId { get; set; }
    public Guid? SubjectId { get; set; }
    public Guid? QuestDependencyId { get; set; }
    public IList<Guid> BnccIds { get; set; } = new List<Guid>();
    public IList<CreateFullQuestStepDto> Steps { get; set; } = new List<CreateFullQuestStepDto>();
}

public class CreateFullQuestCommandHandler : IRequestHandler<CreateFullQuestCommand, IdResponseDto>
{
    private readonly IApplicationDbContext _context;

    public CreateFullQuestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IdResponseDto> Handle(CreateFullQuestCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar Quest Dependency se fornecida
        Domain.Entities.Quest? questDependency = null;
        if (request.QuestDependencyId.HasValue)
        {
            questDependency = await _context.Quests
                .FirstOrDefaultAsync(e => e.Id == request.QuestDependencyId.Value, cancellationToken);
            Guard.Against.NotFound(request.QuestDependencyId.Value, questDependency, nameof(Domain.Entities.Quest));
        }

        // 2. Buscar as entidades BNCC
        var bnccEntities = await GetBnccsByIdsAsync(request.BnccIds, cancellationToken);

        // 3. Criar a Quest
        var quest = new Domain.Entities.Quest(
            request.Name,
            request.Description,
            request.UsageTemplate,
            request.Type,
            request.MaxPlayers,
            request.TotalQuestSteps,
            request.CombatDifficulty)
        {
            GameId = request.GameId,
            GradeId = request.GradeId,
            SubjectId = request.SubjectId,
            QuestDependency = questDependency
        };

        // 4. Associar BNCCs à Quest
        foreach (var bnccEntity in bnccEntities)
        {
            quest.BnccQuests.Add(new BnccQuest
            {
                Quest = quest,
                Bncc = bnccEntity
            });
        }

        // 5. Criar os Steps, Contents e Answers
        foreach (var stepDto in request.Steps)
        {
            // Buscar entidades relacionadas ao step
            var npcEntities = await GetNpcsByIdsAsync(stepDto.NpcIds, cancellationToken);
            var mediaEntities = await GetMediasByIdsAsync(stepDto.MediaIds, cancellationToken);
            var itemEntities = await GetItemsByIdsAsync(stepDto.ItemIds, cancellationToken);

            // Criar o QuestStep
            var questStep = new Domain.Entities.QuestStep(
                stepDto.Name,
                stepDto.Description,
                stepDto.Order,
                stepDto.NpcType,
                stepDto.NpcBehavior,
                stepDto.QuestStepType)
            {
                Quest = quest
            };

            // Criar os Contents para este Step
            foreach (var contentDto in stepDto.Contents)
            {
                var questStepContent = new Domain.Entities.QuestStepContent(
                    contentDto.QuestStepContentType,
                    contentDto.QuestionType,
                    contentDto.Description,
                    contentDto.ExpectedAnswer.ToJsonObject(),
                    contentDto.Weight)
                {
                    QuestStep = questStep
                };

                questStep.Contents.Add(questStepContent);
            }

            // Associar NPCs ao Step
            foreach (var npcEntity in npcEntities)
            {
                questStep.QuestStepNpcs.Add(new QuestStepNpc
                {
                    QuestStep = questStep,
                    Npc = npcEntity
                });
            }

            // Associar Medias ao Step
            foreach (var mediaEntity in mediaEntities)
            {
                questStep.QuestStepMedias.Add(new QuestStepMedia
                {
                    QuestStep = questStep,
                    Media = mediaEntity
                });
            }

            // Associar Items ao Step
            foreach (var itemEntity in itemEntities)
            {
                questStep.QuestStepItems.Add(new QuestStepItem
                {
                    QuestStep = questStep,
                    Item = itemEntity
                });
            }

            // Adicionar o step à quest
            quest.QuestSteps.Add(questStep);
        }

        // 6. Salvar tudo no banco
        _context.Quests.Add(quest);
        await _context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(quest.Id);
    }

    #region Helper Methods

    private async Task<List<Domain.Entities.Bncc>> GetBnccsByIdsAsync(IList<Guid> bnccIds, CancellationToken cancellationToken)
    {
        if (bnccIds == null || !bnccIds.Any())
            return new List<Domain.Entities.Bncc>();

        var entities = await _context.Bnccs
            .Where(e => bnccIds.Contains(e.Id))
            .ToListAsync(cancellationToken);

        var missingIds = bnccIds.Except(entities.Select(e => e.Id)).ToList();
        if (missingIds.Any())
            throw new NotFoundException(nameof(Domain.Entities.Bncc), string.Join(", ", missingIds));

        return entities;
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

    #endregion
}
