using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Subject.UpdateSubject;

public record UpdateSubjectCommand : IRequest<CreatedResponseDto>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class UpdateSubjectCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateSubjectCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Subjects
            .Where(a => a.Id == request.Id)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.Name = request.Name ?? entity.Name;
        entity.Description = request.Description ?? entity.Description;

        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}