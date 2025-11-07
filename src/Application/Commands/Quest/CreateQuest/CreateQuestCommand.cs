using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Common;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Quest.CreateQuest;

public record CreateQuestCommand(
    string Name,
    string Description,
    QuestUsageTemplate UsageTemplate,
    QuestType Type,
    int MaxPlayers,
    int TotalQuestSteps,
    CombatDifficulty CombatDifficulty,
    Guid? GameId,
    Guid? GradeId,
    Guid? SubjectId,
    Guid? QuestDependencyId = null,
    IList<Guid>? ProficiencyIds = null) : IRequest<IdResponseDto>;

public class CreateQuestCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateQuestCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateQuestCommand request, CancellationToken cancellationToken)
    {
        // Validate and retrieve the related entities

        // Domain.Entities.Game? game = null;
        // Domain.Entities.Grade? grade = null;
        // Domain.Entities.Subject? subject = null;

        // if (request.GameId.HasValue && request.GameId != Guid.Empty){
        //     game = await GetEntityByIdAsync(context.Games, request.GameId, nameof(Domain.Entities.Game),
        //     cancellationToken);
        // }
        
        // if (request.GradeId.HasValue && request.GradeId != Guid.Empty) {
        //     grade = await GetEntityByIdAsync(context.Grades, request.GradeId, nameof(Domain.Entities.Grade),
        //     cancellationToken);
        // }

        // if (request.SubjectId.HasValue && request.SubjectId != Guid.Empty) {
        //     subject = await GetEntityByIdAsync(context.Subjects, request.SubjectId, nameof(Domain.Entities.Subject),
        //     cancellationToken);
        // }

        Domain.Entities.Quest? questDependency = null;
        if (request.QuestDependencyId.HasValue)
        {
            questDependency = await GetEntityByIdAsync<Domain.Entities.Quest>(context.Quests,
                request.QuestDependencyId.Value, nameof(Domain.Entities.Quest), cancellationToken);
        }

        var proficiencyEntities = await GetProficienciesByIdsAsync(context, request.ProficiencyIds, cancellationToken);

        // Create the Quest entity
        var quest = new Domain.Entities.Quest(
            request.Name,
            request.Description,
            request.UsageTemplate,
            request.Type,
            request.MaxPlayers,
            request.TotalQuestSteps,
            request.CombatDifficulty)
        {
        //     Game = game,
        //     Grade = grade,
        //     Subject = subject,
            QuestDependency = questDependency
        };

        // Associate the Proficiencies with the Quest
        foreach (var proficiencyEntity in proficiencyEntities)
        {
            quest.QuestProficiencies.Add(new QuestProficiency { Quest = quest, Proficiency = proficiencyEntity });
        }

        context.Quests.Add(quest);
        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(quest.Id);
    }

    private async Task<TEntity> GetEntityByIdAsync<TEntity>(DbSet<TEntity> dbSet, Guid id, string entityName,
        CancellationToken cancellationToken) where TEntity : BaseAuditableEntity
    {
        var entity = await dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        Guard.Against.NotFound(id, entity);
        return entity;
    }

    private async Task<List<Domain.Entities.Proficiency>> GetProficienciesByIdsAsync(
        IApplicationDbContext context,
        IList<Guid>? proficiencyIds,
        CancellationToken cancellationToken)
    {
        if (proficiencyIds is not { Count: > 0 }) return [];

        var proficiencyEntities = await context.Proficiencies
            .Where(p => proficiencyIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        var missingProficiencyIds = proficiencyIds.Except(proficiencyEntities.Select(p => p.Id)).ToList();
        if (missingProficiencyIds.Count != 0)
        {
            throw new NotFoundException(nameof(Domain.Entities.Proficiency), string.Join(", ", missingProficiencyIds));
        }

        return proficiencyEntities;
    }
}