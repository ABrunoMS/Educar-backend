using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Proficiency.UpdateProficiency;

public class UpdateProficiencyCommand : IRequest<IdResponseDto>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Purpose { get; set; }
}

public class UpdateProficiencyCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateProficiencyCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(UpdateProficiencyCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Proficiencies
            .Where(a => a.Id == request.Id)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.Name = request.Name ?? entity.Name;
        entity.Description = request.Description ?? entity.Description;
        entity.Purpose = request.Purpose ?? entity.Purpose;

        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}