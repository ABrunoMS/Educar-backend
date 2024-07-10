using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Account.UpdateAccount;

public record UpdateAccountCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? RegistrationNumber { get; set; }
    public UserRole Role { get; set; }
    public decimal AverageScore { get; set; }
    public decimal EventAverageScore { get; set; }
    public int Stars { get; set; }
}

public class UpdateAccountCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateAccountCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Accounts
            .Where(a => a.Id == request.Id)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        if (request.Name != null) entity.Name = request.Name;
        if (request.RegistrationNumber != null) entity.RegistrationNumber = request.RegistrationNumber;
        entity.AverageScore = request.AverageScore;
        entity.EventAverageScore = request.EventAverageScore;
        entity.Stars = request.Stars;
        entity.Role = request.Role;

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}