using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace Educar.Backend.Application.Commands.Client.CreateClient;

// Nenhuma mudança aqui
public record CreateClientCommand : IRequest<IdResponseDto>
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("partner")]
    public string Partner { get; init; } = string.Empty;

    [JsonPropertyName("contacts")]
    public string Contacts { get; init; } = string.Empty;

    [JsonPropertyName("contract")]
    public string Contract { get; init; } = string.Empty;

    [JsonPropertyName("validity")]
    public string Validity { get; init; } = string.Empty;

    [JsonPropertyName("signatureDate")]
    public string SignatureDate { get; init; } = string.Empty;

    [JsonPropertyName("implantationDate")]
    public string? ImplantationDate { get; init; }

    [JsonPropertyName("totalAccounts")]
    public int TotalAccounts { get; init; }

    [JsonPropertyName("secretary")]
    public string Secretary { get; init; } = string.Empty;

    [JsonPropertyName("subSecretary")]
    public string SubSecretary { get; init; } = string.Empty;

    [JsonPropertyName("regional")]
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