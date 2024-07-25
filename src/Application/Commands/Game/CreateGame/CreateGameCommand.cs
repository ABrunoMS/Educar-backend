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
        var subjectEntities = new List<Domain.Entities.Subject>();
        if (request.SubjectIds != null && request.SubjectIds.Count != 0)
        {
            subjectEntities = await context.Subjects
                .Where(c => request.SubjectIds.Contains(c.Id))
                .ToListAsync(cancellationToken);

            var missingSubjectIds = request.SubjectIds.Except(subjectEntities.Select(c => c.Id)).ToList();
            if (missingSubjectIds.Count != 0)
            {
                throw new NotFoundException(nameof(Domain.Entities.Subject), string.Join(", ", missingSubjectIds));
            }
        }

        var entity = new Domain.Entities.Game(request.Name, request.Description, request.Lore, request.Purpose);


        foreach (var subjectEntity in subjectEntities)
        {
            entity.GameSubjects.Add(new GameSubject { Game = entity, Subject = subjectEntity });
        }

        context.Games.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}