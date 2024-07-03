using Educar.Backend.Application.Interfaces;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Contract.CreateAccountType;

public class CreateContractCommand(
    int contractDurationInYears,
    DateTimeOffset contractSigningDate,
    DateTimeOffset implementationDate,
    int totalAccounts,
    ContractStatus status)
    : IRequest<CreatedResponseDto>
{
    public int ContractDurationInYears { get; set; } = contractDurationInYears;
    public DateTimeOffset ContractSigningDate { get; set; } = contractSigningDate;
    public DateTimeOffset ImplementationDate { get; set; } = implementationDate;
    public int TotalAccounts { get; set; } = totalAccounts;
    public int? RemainingAccounts { get; set; }
    public string? DeliveryReport { get; set; }
    public ContractStatus Status { get; set; } = status;
}

public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, CreatedResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateContractCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

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

        _context.Contracts.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}