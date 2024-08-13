using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Quest.UpdateQuest;

public record UpdateQuestCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public QuestUsageTemplate? UsageTemplate { get; set; }
    public QuestType? Type { get; set; }
    public int? MaxPlayers { get; set; }
    public int? TotalQuestSteps { get; set; }
    public CombatDifficulty? CombatDifficulty { get; set; }
    public Guid? GradeId { get; set; }
    public Guid? SubjectId { get; set; }
    public Guid? QuestDependencyId { get; set; }
    public IList<Guid>? ProficiencyIds { get; set; }
}

public class UpdateQuestCommandHandler : IRequestHandler<UpdateQuestCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateQuestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateQuestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Quests
            .Include(q => q.QuestProficiencies)
            .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        UpdateQuestProperties(entity, request);

        await UpdateQuestProficiencies(_context, entity, request.ProficiencyIds, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private void UpdateQuestProperties(Domain.Entities.Quest entity, UpdateQuestCommand request)
    {
        if (request.Name != null) entity.Name = request.Name;
        if (request.Description != null) entity.Description = request.Description;
        if (request.UsageTemplate.HasValue) entity.UsageTemplate = request.UsageTemplate.Value;
        if (request.Type.HasValue) entity.Type = request.Type.Value;
        if (request.MaxPlayers.HasValue) entity.MaxPlayers = request.MaxPlayers.Value;
        if (request.TotalQuestSteps.HasValue) entity.TotalQuestSteps = request.TotalQuestSteps.Value;
        if (request.CombatDifficulty.HasValue) entity.CombatDifficulty = request.CombatDifficulty.Value;

        if (request.GradeId.HasValue)
        {
            var grade = _context.Grades.Find(request.GradeId.Value);
            Guard.Against.NotFound(request.GradeId.Value, grade);
            entity.Grade = grade;
        }

        if (request.SubjectId.HasValue)
        {
            var subject = _context.Subjects.Find(request.SubjectId.Value);
            Guard.Against.NotFound(request.SubjectId.Value, subject);
            entity.Subject = subject;
        }

        if (!request.QuestDependencyId.HasValue) return;

        var questDependency = _context.Quests.Find(request.QuestDependencyId.Value);
        Guard.Against.NotFound(request.QuestDependencyId.Value, questDependency);
        entity.QuestDependency = questDependency;
    }

    private async Task UpdateQuestProficiencies(IApplicationDbContext context, Domain.Entities.Quest entity,
        IList<Guid>? proficiencyIds, CancellationToken cancellationToken)
    {
        if (proficiencyIds == null || proficiencyIds.Count == 0)
        {
            return;
        }

        var proficiencyEntities = await context.Proficiencies
            .Where(p => proficiencyIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        var missingProficiencyIds = proficiencyIds.Except(proficiencyEntities.Select(p => p.Id)).ToList();
        if (missingProficiencyIds.Any())
        {
            throw new NotFoundException(nameof(Proficiency), string.Join(", ", missingProficiencyIds));
        }

        var allQuestProficiencies = await context.QuestProficiencies
            .IgnoreQueryFilters()
            .Where(qp => qp.QuestId == entity.Id)
            .ToListAsync(cancellationToken);

        UpdateProficiencyRelationships(entity, allQuestProficiencies, proficiencyEntities, proficiencyIds);
    }

    private void UpdateProficiencyRelationships(Domain.Entities.Quest entity,
        List<QuestProficiency> allQuestProficiencies,
        List<Domain.Entities.Proficiency> proficiencyEntities, IList<Guid> proficiencyIds)
    {
        var currentProficiencyIds = allQuestProficiencies.Where(qp => !qp.IsDeleted)
            .Select(qp => qp.ProficiencyId).ToList();
        var proficienciesToAdd = proficiencyIds.Except(currentProficiencyIds).ToList();
        var proficienciesToRemove = currentProficiencyIds.Except(proficiencyIds).ToList();

        foreach (var proficiencyId in proficienciesToAdd)
        {
            var existingQuestProficiency =
                allQuestProficiencies.FirstOrDefault(qp =>
                    qp.ProficiencyId == proficiencyId && qp.IsDeleted);
            if (existingQuestProficiency != null)
            {
                existingQuestProficiency.IsDeleted = false;
                existingQuestProficiency.DeletedAt = null;
            }
            else
            {
                var proficiencyEntity = proficiencyEntities.First(p => p.Id == proficiencyId);
                entity.QuestProficiencies.Add(new QuestProficiency
                    { QuestId = entity.Id, ProficiencyId = proficiencyEntity.Id });
            }
        }

        foreach (var questProficiency in proficienciesToRemove.Select(proficiencyId =>
                     allQuestProficiencies.First(qp => qp.ProficiencyId == proficiencyId)))
        {
            questProficiency.IsDeleted = true;
            questProficiency.DeletedAt = DateTimeOffset.UtcNow;
        }
    }
}