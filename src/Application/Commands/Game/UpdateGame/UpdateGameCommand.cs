using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Application.Commands.Game.UpdateGame;

public record UpdateGameCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Lore { get; set; }
    public string? Purpose { get; set; }
    public IList<Guid> SubjectIds { get; set; } = new List<Guid>();
    public IList<Guid> ProficiencyGroupIds { get; set; } = new List<Guid>();
}

public class UpdateGameCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateGameCommand, Unit>
{
    public async Task<Unit> Handle(UpdateGameCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Games
            .Include(g => g.GameSubjects)
            .Include(g => g.GameProficiencyGroups)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        UpdateGameProperties(entity, request);

        await UpdateGameSubjects(context, entity, request.SubjectIds, cancellationToken);
        await UpdateGameProficiencyGroups(context, entity, request.ProficiencyGroupIds, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private void UpdateGameProperties(Domain.Entities.Game entity, UpdateGameCommand request)
    {
        if (request.Name != null) entity.Name = request.Name;
        if (request.Description != null) entity.Description = request.Description;
        if (request.Lore != null) entity.Lore = request.Lore;
        if (request.Purpose != null) entity.Purpose = request.Purpose;
    }

    private async Task UpdateGameSubjects(IApplicationDbContext context, Domain.Entities.Game entity,
        IList<Guid> subjectIds, CancellationToken cancellationToken)
    {
        var subjectEntities = await context.Subjects
            .Where(s => subjectIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        var missingSubjectIds = subjectIds.Except(subjectEntities.Select(s => s.Id)).ToList();
        if (missingSubjectIds.Count != 0)
        {
            throw new NotFoundException(nameof(Subject), string.Join(", ", missingSubjectIds));
        }

        var allGameSubjects = await context.GameSubjects
            .IgnoreQueryFilters()
            .Where(gs => gs.GameId == entity.Id)
            .ToListAsync(cancellationToken);

        UpdateSubjectRelationships(entity, allGameSubjects, subjectEntities, subjectIds);
    }

    private void UpdateSubjectRelationships(Domain.Entities.Game entity, List<GameSubject> allGameSubjects,
        List<Domain.Entities.Subject> subjectEntities, IList<Guid> subjectIds)
    {
        var currentSubjectIds = allGameSubjects.Where(gs => !gs.IsDeleted).Select(gs => gs.SubjectId).ToList();
        var subjectsToAdd = subjectIds.Except(currentSubjectIds).ToList();
        var subjectsToRemove = currentSubjectIds.Except(subjectIds).ToList();

        foreach (var subjectId in subjectsToAdd)
        {
            var existingGameSubject = allGameSubjects.FirstOrDefault(gs => gs.SubjectId == subjectId && gs.IsDeleted);
            if (existingGameSubject != null)
            {
                existingGameSubject.IsDeleted = false;
                existingGameSubject.DeletedAt = null;
            }
            else
            {
                var subjectEntity = subjectEntities.First(s => s.Id == subjectId);
                entity.GameSubjects.Add(new GameSubject { GameId = entity.Id, SubjectId = subjectEntity.Id });
            }
        }

        foreach (var gameSubject in subjectsToRemove.Select(subjectId =>
                     allGameSubjects.First(gs => gs.SubjectId == subjectId)))
        {
            gameSubject.IsDeleted = true;
            gameSubject.DeletedAt = DateTimeOffset.UtcNow;
        }
    }

    private async Task UpdateGameProficiencyGroups(IApplicationDbContext context, Domain.Entities.Game entity,
        IList<Guid> proficiencyGroupIds, CancellationToken cancellationToken)
    {
        var proficiencyGroupEntities = await context.ProficiencyGroups
            .Where(pg => proficiencyGroupIds.Contains(pg.Id))
            .ToListAsync(cancellationToken);

        var missingProficiencyGroupIds =
            proficiencyGroupIds.Except(proficiencyGroupEntities.Select(pg => pg.Id)).ToList();
        if (missingProficiencyGroupIds.Count != 0)
        {
            throw new NotFoundException(nameof(ProficiencyGroup), string.Join(", ", missingProficiencyGroupIds));
        }

        var allGameProficiencyGroups = await context.GameProficiencyGroups
            .IgnoreQueryFilters()
            .Where(gpg => gpg.GameId == entity.Id)
            .ToListAsync(cancellationToken);

        UpdateProficiencyGroupRelationships(entity, allGameProficiencyGroups, proficiencyGroupEntities,
            proficiencyGroupIds);
    }

    private void UpdateProficiencyGroupRelationships(Domain.Entities.Game entity,
        List<GameProficiencyGroup> allGameProficiencyGroups,
        List<Domain.Entities.ProficiencyGroup> proficiencyGroupEntities, IList<Guid> proficiencyGroupIds)
    {
        var currentProficiencyGroupIds = allGameProficiencyGroups.Where(gpg => !gpg.IsDeleted)
            .Select(gpg => gpg.ProficiencyGroupId).ToList();
        var proficiencyGroupsToAdd = proficiencyGroupIds.Except(currentProficiencyGroupIds).ToList();
        var proficiencyGroupsToRemove = currentProficiencyGroupIds.Except(proficiencyGroupIds).ToList();

        foreach (var proficiencyGroupId in proficiencyGroupsToAdd)
        {
            var existingGameProficiencyGroup =
                allGameProficiencyGroups.FirstOrDefault(gpg =>
                    gpg.ProficiencyGroupId == proficiencyGroupId && gpg.IsDeleted);
            if (existingGameProficiencyGroup != null)
            {
                existingGameProficiencyGroup.IsDeleted = false;
                existingGameProficiencyGroup.DeletedAt = null;
            }
            else
            {
                var proficiencyGroupEntity = proficiencyGroupEntities.First(pg => pg.Id == proficiencyGroupId);
                entity.GameProficiencyGroups.Add(new GameProficiencyGroup
                    { GameId = entity.Id, ProficiencyGroupId = proficiencyGroupEntity.Id });
            }
        }

        foreach (var gameProficiencyGroup in proficiencyGroupsToRemove.Select(proficiencyGroupId =>
                     allGameProficiencyGroups.First(gpg => gpg.ProficiencyGroupId == proficiencyGroupId)))
        {
            gameProficiencyGroup.IsDeleted = true;
            gameProficiencyGroup.DeletedAt = DateTimeOffset.UtcNow;
        }
    }
}