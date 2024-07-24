using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Subject.CreateSubject;

public class CreateSubjectCommand(string Name, string Description) : IRequest<CreatedResponseDto>
{
    public string Name { get; set; } = Name;
    public string Description { get; set; } = Description;
}

public class CreateSubjectCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateSubjectCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Subject(request.Name, request.Description);

        context.Subjects.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}