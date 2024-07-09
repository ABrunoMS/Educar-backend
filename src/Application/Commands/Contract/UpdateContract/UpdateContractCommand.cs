using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Contract.UpdateContract;

public record UpdateContractCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public int ContractDurationInYears { get; init; }
    public DateTimeOffset ContractSigningDate { get; init; }
    public DateTimeOffset ImplementationDate { get; init; }
    public int TotalAccounts { get; init; }
    public int RemainingAccounts { get; init; }
    public string? DeliveryReport { get; init; }
    public ContractStatus Status { get; init; }
}

public class UpdateContractCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateContractCommand, Unit>
{
    public async Task<Unit> Handle(UpdateContractCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Contracts
            .Where(a => a.Id == request.Id)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.ContractDurationInYears = request.ContractDurationInYears;
        entity.ContractSigningDate = request.ContractSigningDate;
        entity.ImplementationDate = request.ImplementationDate;
        entity.TotalAccounts = request.TotalAccounts;
        entity.RemainingAccounts = request.RemainingAccounts;
        entity.DeliveryReport = request.DeliveryReport;
        entity.Status = request.Status;

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}