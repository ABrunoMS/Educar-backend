using Educar.Backend.Application.Common.Interfaces;
using MediatR;
using System.Collections.Generic;
using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Ardalis.GuardClauses;
using System.Linq;

namespace Educar.Backend.Application.Commands.Client.UpdateClient;

public record UpdateRegionalDto
{
    public Guid? Id { get; init; } // Se null/vazio, é uma nova regional
    public string Name { get; init; } = string.Empty;
}

// DTO para Subsecretaria no Update
public record UpdateSubsecretariaDto
{
    public Guid? Id { get; init; } // Se null/vazio, é uma nova subsecretaria
    public string Name { get; init; } = string.Empty;
    public List<UpdateRegionalDto>? Regionais { get; init; }
}

public record UpdateClientCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Partner { get; init; }
    public string? Contacts { get; init; }
    public string? Contract { get; init; }
    public string? Validity { get; init; }
    public string? SignatureDate { get; init; }
    public string? ImplantationDate { get; init; }
    public int? TotalAccounts { get; init; }

    // MUDANÇA AQUI: Em vez de List<Guid>, usamos a estrutura completa
    public List<UpdateSubsecretariaDto>? Subsecretarias { get; init; } 
    
    public List<Guid>? ProductIds { get; init; }
    public List<Guid>? ContentIds { get; init; }
    public Guid? MacroRegionId { get; init; }
}

public class UpdateClientCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateClientCommand, Unit>
{
    public async Task<Unit> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        // 1. Busca a entidade incluindo a hierarquia completa (Client -> Sub -> Regional)
        var entity = await context.Clients
            .Include(c => c.ClientProducts)
            .Include(c => c.ClientContents)
            .Include(c => c.Subsecretarias)
                .ThenInclude(s => s.Regionais) // Importante incluir Regionais
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        // 2. Atualiza propriedades simples do Cliente
        entity.Name = request.Name ?? entity.Name;
        entity.Description = request.Description ?? entity.Description;
        entity.Partner = request.Partner ?? entity.Partner;
        entity.Contacts = request.Contacts ?? entity.Contacts;
        entity.Contract = request.Contract ?? entity.Contract;
        entity.Validity = request.Validity ?? entity.Validity;
        entity.SignatureDate = request.SignatureDate ?? entity.SignatureDate;
        entity.ImplantationDate = request.ImplantationDate ?? entity.ImplantationDate;
        entity.TotalAccounts = request.TotalAccounts ?? entity.TotalAccounts;
        entity.MacroRegionId = request.MacroRegionId ?? entity.MacroRegionId;

        // 3. Atualiza Subsecretarias e Regionais (Lógica complexa de sincronização)
        if (request.Subsecretarias != null)
        {
            UpdateSubsecretarias(entity, request.Subsecretarias);
        }

        // 4. Atualiza Products (Mantém sua lógica original que estava correta para N-N)
        if (request.ProductIds != null)
        {
            UpdateJunctionTable(entity.ClientProducts, request.ProductIds, cp => cp.ProductId, id => new ClientProduct { ProductId = id });
        }

        // 5. Atualiza Contents
        if (request.ContentIds != null)
        {
            UpdateJunctionTable(entity.ClientContents, request.ContentIds, cc => cc.ContentId, id => new ClientContent { ContentId = id });
        }

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    private void UpdateSubsecretarias(Domain.Entities.Client client, List<UpdateSubsecretariaDto> dtos)
    {
        var currentSubs = client.Subsecretarias.ToList();

        // A. REMOVER: Subsecretarias que estão no banco mas não vieram no request
        // Consideramos que o DTO tem ID. Se o ID não veio na lista, deleta.
        var dtoIds = dtos.Where(d => d.Id.HasValue).Select(d => d.Id!.Value).ToList();
        var subsToRemove = currentSubs.Where(s => !dtoIds.Contains(s.Id)).ToList();

        foreach (var sub in subsToRemove)
        {
            // Opcional: Se houver escolas vinculadas, isso pode dar erro de FK. 
            // O ideal seria verificar antes ou usar Cascade Delete no banco.
            client.Subsecretarias.Remove(sub);
            // context.Subsecretarias.Remove(sub); // Força a deleção se necessário
        }

        // B. ADICIONAR ou ATUALIZAR
        foreach (var dto in dtos)
        {
            if (dto.Id.HasValue && dto.Id != Guid.Empty)
            {
                // -- ATUALIZAR EXISTENTE --
                var existingSub = currentSubs.FirstOrDefault(s => s.Id == dto.Id.Value);
                if (existingSub != null)
                {
                    existingSub.Name = dto.Name;
                    // Sincronizar as Regionais desta subsecretaria
                    UpdateRegionais(existingSub, dto.Regionais);
                }
            }
            else
            {
                // -- ADICIONAR NOVO --
                var newSub = new Domain.Entities.Subsecretaria
                {
                    Name = dto.Name,
                    // IDs são gerados automaticamente pelo Guid se configurado, ou pelo banco
                };
                
                // Adiciona as regionais da nova subsecretaria
                if (dto.Regionais != null)
                {
                    foreach(var regDto in dto.Regionais)
                    {
                        newSub.Regionais.Add(new Domain.Entities.Regional { Name = regDto.Name });
                    }
                }

                client.Subsecretarias.Add(newSub);
            }
        }
    }

    private void UpdateRegionais(Domain.Entities.Subsecretaria sub, List<UpdateRegionalDto>? regionalDtos)
    {
        if (regionalDtos == null) return;

        var currentRegionais = sub.Regionais.ToList();
        var dtoIds = regionalDtos.Where(r => r.Id.HasValue).Select(r => r.Id!.Value).ToList();

        // 1. Remover Regionais
        var regionaisToRemove = currentRegionais.Where(r => !dtoIds.Contains(r.Id)).ToList();
        foreach (var reg in regionaisToRemove)
        {
            sub.Regionais.Remove(reg);
        }

        // 2. Adicionar ou Atualizar Regionais
        foreach (var dto in regionalDtos)
        {
            if (dto.Id.HasValue && dto.Id != Guid.Empty)
            {
                // Editar
                var existingReg = currentRegionais.FirstOrDefault(r => r.Id == dto.Id.Value);
                if (existingReg != null)
                {
                    existingReg.Name = dto.Name;
                }
            }
            else
            {
                // Novo
                sub.Regionais.Add(new Domain.Entities.Regional { Name = dto.Name });
            }
        }
    }

    // Seu método genérico original (mantido para Products e Contents)
    private void UpdateJunctionTable<TEntity, TKey>(
        ICollection<TEntity> currentItems,
        ICollection<TKey> newItemIds,
        Func<TEntity, TKey> keySelector,
        Func<TKey, TEntity> createFactory)
        where TEntity : class
        where TKey : IEquatable<TKey>
    {
        var currentIds = currentItems.Select(keySelector).ToList();
        var idsToAdd = newItemIds.Except(currentIds).ToList();
        foreach (var id in idsToAdd)
        {
            currentItems.Add(createFactory(id));
        }

        var idsToRemove = currentIds.Except(newItemIds).ToList();
        var itemsToRemove = currentItems.Where(item => idsToRemove.Contains(keySelector(item))).ToList();
        foreach (var item in itemsToRemove)
        {
            currentItems.Remove(item);
        }
    }
}
