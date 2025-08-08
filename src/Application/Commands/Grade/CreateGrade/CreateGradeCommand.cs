using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Grade.CreateGradeCommand;

public record CreateGradeCommand(string Name, string Description) : IRequest<IdResponseDto>;

public class CreateGradeCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateGradeCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateGradeCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Grade(request.Name, request.Description);
        context.Grades.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}