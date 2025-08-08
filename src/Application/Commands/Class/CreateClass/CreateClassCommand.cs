using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Class.CreateClass;

public record CreateClassCommand(string Name, string Description, ClassPurpose Purpose, Guid SchoolId)
    : IRequest<IdResponseDto>
{
    public string Name { get; set; } = Name;
    public string Description { get; set; } = Description;
    public ClassPurpose Purpose { get; set; } = Purpose;
    public Guid SchoolId { get; set; } = SchoolId;
}

public class CreateClassCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateClassCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        var school = await context.Schools.FindAsync([request.SchoolId], cancellationToken: cancellationToken);
        if (school == null) throw new NotFoundException(nameof(School), request.SchoolId.ToString());

        var entity = new Domain.Entities.Class(request.Name, request.Description, request.Purpose)
        {
            School = school
        };

        context.Classes.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}