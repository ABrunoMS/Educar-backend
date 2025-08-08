using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Subject.UpdateSubject;

public record UpdateSubjectCommand : IRequest<IdResponseDto>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class UpdateSubjectCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateSubjectCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Subjects
            .Where(a => a.Id == request.Id)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.Name = request.Name ?? entity.Name;
        entity.Description = request.Description ?? entity.Description;

        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}