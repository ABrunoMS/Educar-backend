using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Models;
using MediatR;

namespace Educar.Backend.Application.Commands.Client.CreateClient;

// Nenhuma mudança aqui
public record CreateClientCommand : IRequest<IdResponseDto>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Partner { get; init; } = string.Empty;
    public string Contacts { get; init; } = string.Empty;
    public string Contract { get; init; } = string.Empty;
    public string Validity { get; init; } = string.Empty;
    public string SignatureDate { get; init; } = string.Empty;
    public string? ImplantationDate { get; init; }
    public int TotalAccounts { get; init; }
    public string Secretary { get; init; } = string.Empty;
    public string SubSecretary { get; init; } = string.Empty;
    public string Regional { get; init; } = string.Empty;
}

// Nenhuma mudança aqui
public class CreateClientCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateClientCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Client(request.Name)
        {
            Description = request.Description,
            Partner = request.Partner,
            Contacts = request.Contacts,
            Contract = request.Contract,
            Validity = request.Validity,
            SignatureDate = request.SignatureDate,
            ImplantationDate = request.ImplantationDate,
            TotalAccounts = request.TotalAccounts,
            Secretary = request.Secretary,
            SubSecretary = request.SubSecretary,
            Regional = request.Regional,
        };

        context.Clients.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return new IdResponseDto(entity.Id);
    }
}