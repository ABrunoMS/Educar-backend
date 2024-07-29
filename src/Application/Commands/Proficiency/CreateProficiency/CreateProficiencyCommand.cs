using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Proficiency.CreateProficiency;

public record CreateProficiencyCommand(string Name, string Description, string Purpose) : IRequest<CreatedResponseDto>;

public class CreateProficiencyCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateProficiencyCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(CreateProficiencyCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Proficiency(request.Name, request.Description, request.Purpose);

        context.Proficiencies.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}