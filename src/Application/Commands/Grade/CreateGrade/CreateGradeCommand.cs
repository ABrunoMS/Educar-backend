using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Grade.CreateGradeCommand;

public record CreateGradeCommand(string Name, string Description) : IRequest<CreatedResponseDto>;

public class CreateGradeCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateGradeCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(CreateGradeCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Grade(request.Name, request.Description);
        context.Grades.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}