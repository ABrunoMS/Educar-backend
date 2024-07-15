using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;
using ValidationException = Educar.Backend.Application.Common.Exceptions.ValidationException;

namespace Educar.Backend.Application.Commands.Account.UpdateAccount;

public record UpdateAccountCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? RegistrationNumber { get; set; }
    public decimal AverageScore { get; set; }
    public decimal EventAverageScore { get; set; }
    public int Stars { get; set; }
    public Guid? SchoolId { get; set; }
}

public class UpdateAccountCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateAccountCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Accounts
            .Where(a => a.Id == request.Id)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        Domain.Entities.School? school = null;
        if (request.SchoolId != null && request.SchoolId != Guid.Empty)
        {
            school = await context.Schools.FindAsync([request.SchoolId], cancellationToken: cancellationToken);
            if (school == null) throw new NotFoundException(nameof(School), request.SchoolId.ToString()!);
        }

        if (entity.Role != UserRole.Admin)
        {
            if (request.SchoolId == null || request.SchoolId == Guid.Empty)
            {
                var exception = new ValidationException();
                exception.Errors.Add("SchoolId", ["School ID is required for non-admin roles."]);
                throw exception;
            }
        }

        if (request.Name != null) entity.Name = request.Name;
        if (request.RegistrationNumber != null) entity.RegistrationNumber = request.RegistrationNumber;
        entity.AverageScore = request.AverageScore;
        entity.EventAverageScore = request.EventAverageScore;
        entity.Stars = request.Stars;
        entity.School = school;

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}