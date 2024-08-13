using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Proficiency.CreateProficiency;

public record CreateProficiencyCommand(string Name, string Description, string Purpose) : IRequest<IdResponseDto>;

public class CreateProficiencyCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateProficiencyCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateProficiencyCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Proficiency(request.Name, request.Description, request.Purpose);

        context.Proficiencies.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}