using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Application.Commands.Class.CreateClass;

public record CreateClassCommand(string Name, string Description, ClassPurpose Purpose, Guid SchoolId)
    : IRequest<IdResponseDto>
{
    public string Name { get; set; } = Name;
    public string Description { get; set; } = Description;
    public ClassPurpose Purpose { get; set; } = Purpose;
    public Guid SchoolId { get; set; } = SchoolId;
    public bool IsActive { get; init; } = true;
    public string? SchoolYear { get; init; }
    public string? SchoolShift { get; init; }
    public List<Guid>? AccountIds { get; init; }
    public List<Guid>? TeacherIds { get; init; }
    public List<Guid>? StudentIds { get; init; }
    public List<Guid> ProductIds { get; init; } = new();
    public List<Guid> ContentIds { get; init; } = new();
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
            IsActive = request.IsActive,
            SchoolYear = request.SchoolYear,
            SchoolShift = request.SchoolShift
        };

        if (request.ProductIds != null && request.ProductIds.Any())
        {
            foreach (var productId in request.ProductIds)
            {
                entity.ClassProducts.Add(new ClassProduct { ProductId = productId });
            }
        }

        if (request.ContentIds != null && request.ContentIds.Any())
        {
            foreach (var contentId in request.ContentIds)
            {
                entity.ClassContents.Add(new ClassContent { ContentId = contentId });
            }
        }

context.Classes.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        // Adiciona os Professores e Alunos
        var allAccountIds = new List<Guid>();
        if (request.TeacherIds?.Any() == true)
            allAccountIds.AddRange(request.TeacherIds);
        if (request.StudentIds?.Any() == true)
            allAccountIds.AddRange(request.StudentIds);

        if (allAccountIds.Any())
        {
            var accountClasses = allAccountIds.Distinct().Select(accountId => new Domain.Entities.AccountClass
            {
                AccountId = accountId,
                ClassId = entity.Id 
            }).ToList();

            context.AccountClasses.AddRange(accountClasses);
            await context.SaveChangesAsync(cancellationToken); // Salva as associações de contas
        }

        return new IdResponseDto(entity.Id);
    }
}