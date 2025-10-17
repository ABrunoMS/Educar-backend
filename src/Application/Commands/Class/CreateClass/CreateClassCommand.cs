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
    public List<string>? Content { get; init; }
    public bool IsActive { get; init; } = true;
    public string? SchoolYear { get; init; }
    public string? SchoolShift { get; init; }
    public List<Guid>? AccountIds { get; init; }
    public List<Guid>? TeacherIds { get; init; }
    public List<Guid>? StudentIds { get; init; }
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
            School = school,
            Content = request.Content ?? new List<string>(),
            IsActive = request.IsActive,
            SchoolYear = request.SchoolYear,
            SchoolShift = request.SchoolShift
        };

        context.Classes.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        // Adiciona os accounts à turma se fornecidos
        var allAccountIds = new List<Guid>();
        
        // Adiciona teacherIds e studentIds se fornecidos
        if (request.TeacherIds?.Any() == true)
            allAccountIds.AddRange(request.TeacherIds);
        if (request.StudentIds?.Any() == true)
            allAccountIds.AddRange(request.StudentIds);
        
        // Fallback para AccountIds se fornecidos
        if (request.AccountIds?.Any() == true)
            allAccountIds.AddRange(request.AccountIds);

        if (allAccountIds.Any())
        {
            var accountClasses = allAccountIds.Distinct().Select(accountId => new Domain.Entities.AccountClass
            {
                AccountId = accountId,
                ClassId = entity.Id
            }).ToList();

            context.AccountClasses.AddRange(accountClasses);
            await context.SaveChangesAsync(cancellationToken);
        }

        return new IdResponseDto(entity.Id);
    }
}