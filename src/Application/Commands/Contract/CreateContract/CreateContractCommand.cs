using Educar.Backend.Application.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Contract.CreateAccountType;

public record CreateContractCommand : IRequest<CreatedResponseDto>
{
    public CreateContractCommand(Guid clientId)
    {
        ClientId = clientId;
    }

    public int ContractDurationInYears { get; init; }
    public DateTimeOffset ContractSigningDate { get; init; }
    public DateTimeOffset ImplementationDate { get; init; }
    public int TotalAccounts { get; init; }
    public int? RemainingAccounts { get; init; }
    public string? DeliveryReport { get; init; }
    public ContractStatus Status { get; init; }
    public Guid ClientId { get; init; }
    public Domain.Entities.Client? Client { get; init; }
}

public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, CreatedResponseDto>
{
    private readonly IApplicationDbContext _context;

    public CreateContractCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreatedResponseDto> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FindAsync([request.ClientId], cancellationToken: cancellationToken);
        // Guard.Against.Null(client, message: $"Client {request.ClientId} not found.");
        if (client == null) throw new NotFoundException(nameof(Client), request.ClientId.ToString());

        var entity = new Domain.Entities.Contract(
            request.ContractDurationInYears,
            request.ContractSigningDate,
            request.ImplementationDate,
            request.TotalAccounts,
            request.Status
        )
        {
            RemainingAccounts = request.RemainingAccounts ?? request.TotalAccounts,
            DeliveryReport = request.DeliveryReport,
            Client = client
        };

        _context.Contracts.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}