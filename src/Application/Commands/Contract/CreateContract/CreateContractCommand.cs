using Educar.Backend.Application.Interfaces;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Contract.CreateAccountType;

public record CreateContractCommand : IRequest<CreatedResponseDto>
{
    public int ContractDurationInYears { get; init; }
    public DateTimeOffset ContractSigningDate { get; init; }
    public DateTimeOffset ImplementationDate { get; init; }
    public int TotalAccounts { get; init; }
    public int? RemainingAccounts { get; init; }
    public string? DeliveryReport { get; init; }
    public ContractStatus Status { get; init; }
}

public class CreateContractCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<CreateContractCommand, CreatedResponseDto>
{
    private readonly IMapper _mapper = mapper;

    public async Task<CreatedResponseDto> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Contract(
            request.ContractDurationInYears,
            request.ContractSigningDate,
            request.ImplementationDate,
            request.TotalAccounts,
            request.Status
        )
        {
            RemainingAccounts = request.RemainingAccounts ?? request.TotalAccounts,
            DeliveryReport = request.DeliveryReport
        };

        context.Contracts.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}