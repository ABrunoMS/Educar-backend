using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Application.Queries;
using Microsoft.EntityFrameworkCore;
using ClientEntity = Educar.Backend.Domain.Entities.Client;

namespace Educar.Backend.Application.Commands.Contract.CreateContract;

public record CreateContractCommand : IRequest<IdResponseDto>
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
    public Guid? ClientId { get; init; }
    public List<Guid> ProductIds { get; init; } = new();
    public List<Guid>? ContentIds { get; init; } = new();
}

public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, IdResponseDto>
{
    private readonly IApplicationDbContext _context;

    public CreateContractCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IdResponseDto> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        ClientEntity? client = null;
        // 1. Busca o Client apenas se um ID foi fornecido
        if (request.ClientId.HasValue)
        {
            client = await _context.Clients.FindAsync(new object[] { request.ClientId.Value }, cancellationToken);
            if (client == null) throw new NotFoundException(nameof(Client), request.ClientId.Value.ToString());
        }

        //var product = await _context.Products.FindAsync(new object[] { request.ProductId }, cancellationToken);
        //if (product == null) throw new NotFoundException(nameof(Product), request.ProductId.ToString());

        
        var entity = new Domain.Entities.Contract(
            request.ContractDurationInYears,
            request.ContractSigningDate,
            request.ImplementationDate,
            request.TotalAccounts,
            request.Status
        )
        {
            // 4. Calcula RemainingAccounts
            RemainingAccounts = request.TotalAccounts,
            DeliveryReport = request.DeliveryReport,
            Client = client, // Atribui o Client (pode ser nulo)
            
        };


       if (request.ProductIds != null && request.ProductIds.Any())
        {
            
            foreach (var productId in request.ProductIds)
            {
                
                entity.ContractProducts.Add(new ContractProduct { ProductId = productId });
            }
        }

        if (request.ContentIds != null && request.ContentIds.Any())
        {
            foreach (var contentId in request.ContentIds)
            {
                
                entity.ContractContents.Add(new ContractContent { ContentId = contentId });
            }
        }

        _context.Contracts.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}