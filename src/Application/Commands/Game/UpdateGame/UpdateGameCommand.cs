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
}

public class UpdateGameCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateGameCommand, Unit>
{
    public async Task<Unit> Handle(UpdateGameCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Games
            .Include(g => g.GameSubjects)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        if (request.Name != null) entity.Name = request.Name;
        if (request.Description != null) entity.Description = request.Description;
        if (request.Lore != null) entity.Lore = request.Lore;
        if (request.Purpose != null) entity.Purpose = request.Purpose;

        // Validate and fetch subjects
        var subjectEntities = await context.Subjects
            .Where(s => request.SubjectIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        var missingSubjectIds = request.SubjectIds.Except(subjectEntities.Select(s => s.Id)).ToList();
        if (missingSubjectIds.Any())
        {
            throw new NotFoundException(nameof(Subject), string.Join(", ", missingSubjectIds));
        }

        // Handle GameSubject relationships
        var allGameSubjects = await context.GameSubjects
            .IgnoreQueryFilters()
            .Where(gs => gs.GameId == entity.Id)
            .ToListAsync(cancellationToken);

        var currentSubjectIds = allGameSubjects.Where(gs => !gs.IsDeleted).Select(gs => gs.SubjectId).ToList();
        var newSubjectIds = request.SubjectIds;

        // Find subjects to add
        var subjectsToAdd = newSubjectIds.Except(currentSubjectIds).ToList();
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
                entity.GameSubjects.Add(new GameSubject { GameId = entity.Id, SubjectId = subjectId });
            }
        }

        // Find subjects to remove (soft delete)
        var subjectsToRemove = currentSubjectIds.Except(newSubjectIds).ToList();
        foreach (var gameSubject in subjectsToRemove.Select(subjectId =>
                     allGameSubjects.First(gs => gs.SubjectId == subjectId)))
        {
            gameSubject.IsDeleted = true;
            gameSubject.DeletedAt = DateTimeOffset.UtcNow;
        }

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    public async Task<bool> BeUniqueName(string name, Guid id, CancellationToken cancellationToken)
    {
        return await context.Games
            .Where(g => g.Id != id)
            .AllAsync(l => l.Name != name, cancellationToken);
    }
}