using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Domain.Entities;
using MediatR;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore; 
using System.Collections.Generic; 

namespace Educar.Backend.Application.Commands.Client.CreateClient;

public record CreateSubsecretariaDto
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("regionais")]
    public List<CreateRegionalDto>? Regionais { get; init; }
}

public record CreateRegionalDto
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}

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

    [JsonPropertyName("subsecretarias")]
    public List<CreateSubsecretariaDto>? Subsecretarias { get; init; }

    [JsonPropertyName("selectedProducts")]
    public List<Guid>? ProductIds { get; init; }

    [JsonPropertyName("selectedContents")]
    public List<Guid>? ContentIds { get; init; }

    [JsonPropertyName("macroRegionId")]
    public Guid? MacroRegionId { get; init; }
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
            MacroRegionId = request.MacroRegionId,
        };

        // Cria as Subsecretarias e Regionais
        if (request.Subsecretarias != null && request.Subsecretarias.Any())
        {
            foreach (var subsecretariaDto in request.Subsecretarias)
            {
                var subsecretaria = new Domain.Entities.Subsecretaria
                {
                    Name = subsecretariaDto.Name,
                    Client = entity
                };

                // Cria as Regionais para esta Subsecretaria
                if (subsecretariaDto.Regionais != null && subsecretariaDto.Regionais.Any())
                {
                    foreach (var regionalDto in subsecretariaDto.Regionais)
                    {
                        var regional = new Domain.Entities.Regional
                        {
                            Name = regionalDto.Name,
                            Subsecretaria = subsecretaria
                        };
                        subsecretaria.Regionais.Add(regional);
                    }
                }

                entity.Subsecretarias.Add(subsecretaria);
            }
        }

        // Adiciona os Produtos selecionados
        if (request.ProductIds != null && request.ProductIds.Any())
        {
            foreach (var productId in request.ProductIds)
            {
                // (Validação se o productId existe pode ser adicionada no Validator)
                entity.ClientProducts.Add(new ClientProduct { ProductId = productId });
            }
        }

        // Adiciona os Conteúdos selecionados
        if (request.ContentIds != null && request.ContentIds.Any())
        {
            foreach (var contentId in request.ContentIds)
            {
                // (Validação de compatibilidade pode ser adicionada no Validator)
                entity.ClientContents.Add(new ClientContent { ContentId = contentId });
            }
        }

        context.Clients.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return new IdResponseDto(entity.Id);
    }
}
