using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Application.Commands.Game.CreateGame;

public record CreateGameCommand(string Name, string Description, string Lore, string Purpose)
    : IRequest<CreatedResponseDto>
{
    public string Name { get; set; } = Name;
    public string Description { get; set; } = Description;
    public string Lore { get; set; } = Lore;
    public string Purpose { get; set; } = Purpose;
    public IList<Guid> SubjectIds { get; set; } = new List<Guid>();
}

public class CreateGameCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateGameCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        // Validate and fetch subjects
        var subjectEntities = await context.Subjects
            .Where(s => request.SubjectIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        var missingSubjectIds = request.SubjectIds.Except(subjectEntities.Select(s => s.Id)).ToList();
        if (missingSubjectIds.Any())
        {
            throw new NotFoundException(nameof(Subject), string.Join(", ", missingSubjectIds));
        }

        // Create the Game entity
        var entity = new Domain.Entities.Game(request.Name, request.Description, request.Lore, request.Purpose);
        context.Games.Add(entity);

        // Save changes to generate the Game ID
        await context.SaveChangesAsync(cancellationToken);

        // Handle GameSubject relationships
        var existingGameSubjects = await context.GameSubjects
            .IgnoreQueryFilters()
            .Where(gs => request.SubjectIds.Contains(gs.SubjectId) && gs.GameId == entity.Id)
            .ToListAsync(cancellationToken);

        foreach (var subject in subjectEntities)
        {
            var existingGameSubject = existingGameSubjects.FirstOrDefault(gs => gs.SubjectId == subject.Id);

            if (existingGameSubject != null)
            {
                if (existingGameSubject.IsDeleted)
                {
                    existingGameSubject.IsDeleted = false;
                    existingGameSubject.DeletedAt = null;
                }
            }
            else
            {
                entity.GameSubjects.Add(new GameSubject { GameId = entity.Id, SubjectId = subject.Id });
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}