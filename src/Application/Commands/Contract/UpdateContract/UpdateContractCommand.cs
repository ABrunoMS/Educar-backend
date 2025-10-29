using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;
using MediatR;
using System.Collections.Generic;
using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Ardalis.GuardClauses;
using System.Linq;
using System;

namespace Educar.Backend.Application.Commands.Contract.UpdateContract;

public record UpdateContractCommand : IRequest<Unit>
{
    // O 'Id' será preenchido a partir da rota
    public Guid Id { get; set; } 
    
    // Todas as outras propriedades são opcionais no update
    public int? ContractDurationInYears { get; init; }
    public DateTimeOffset? ContractSigningDate { get; init; }
    public DateTimeOffset? ImplementationDate { get; init; }
    public int? TotalAccounts { get; init; }
    public string? DeliveryReport { get; init; }
    public ContractStatus? Status { get; init; }
    
    // ClientId opcional
    public Guid? ClientId { get; init; }
    
    // Novas listas de IDs
    public List<Guid>? ProductIds { get; init; }
    public List<Guid>? ContentIds { get; init; }
}

public class UpdateContractCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateContractCommand, Unit>
{
    public async Task<Unit> Handle(UpdateContractCommand request, CancellationToken cancellationToken)
    {
        // 1. Busca a entidade principal E suas listas de ligação atuais
        var entity = await context.Contracts
            .Include(c => c.ContractProducts)
            .Include(c => c.ContractContents)
            .Include(c => c.Client) // Inclui o Cliente
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        // 2. Atualiza as propriedades simples
        entity.ContractDurationInYears = request.ContractDurationInYears ?? entity.ContractDurationInYears;
        entity.ContractSigningDate = request.ContractSigningDate ?? entity.ContractSigningDate;
        entity.ImplementationDate = request.ImplementationDate ?? entity.ImplementationDate;
        entity.DeliveryReport = request.DeliveryReport ?? entity.DeliveryReport;
        entity.Status = request.Status ?? entity.Status;

        // 3. Lógica especial para TotalAccounts (recalcula RemainingAccounts)
        if (request.TotalAccounts.HasValue)
        {
            var accountsInUse = entity.TotalAccounts - (entity.RemainingAccounts ?? 0);
            entity.TotalAccounts = request.TotalAccounts.Value;
            entity.RemainingAccounts = entity.TotalAccounts - accountsInUse;
        }

        // 4. Atualiza o ClientId (se for alterado)
        if (request.ClientId.HasValue)
        {
            entity.ClientId = request.ClientId.Value == Guid.Empty ? null : request.ClientId.Value;
        }

        // 5. Atualiza as tabelas de ligação (Products)
        if (request.ProductIds != null)
        {
            UpdateJunctionTable(entity.ContractProducts, request.ProductIds, cp => cp.ProductId, id => new ContractProduct { ProductId = id });
        }

        // 6. Atualiza as tabelas de ligação (Contents)
        if (request.ContentIds != null)
        {
            UpdateJunctionTable(entity.ContractContents, request.ContentIds, cc => cc.ContentId, id => new ContractContent { ContentId = id });
        }

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    // Método genérico para atualizar tabelas de ligação (N-N)
    private void UpdateJunctionTable<TEntity, TKey>(
        ICollection<TEntity> currentItems, 
        ICollection<TKey> newItemIds, 
        Func<TEntity, TKey> keySelector, 
        Func<TKey, TEntity> createFactory) 
        where TEntity : class 
        where TKey : IEquatable<TKey>
    {
        var currentIds = currentItems.Select(keySelector).ToList();
        
        // Adicionar novos
        var idsToAdd = newItemIds.Except(currentIds).ToList();
        foreach (var id in idsToAdd)
        {
            currentItems.Add(createFactory(id));
        }

        // Remover antigos
        var idsToRemove = currentIds.Except(newItemIds).ToList();
        var itemsToRemove = currentItems.Where(item => idsToRemove.Contains(keySelector(item))).ToList();
        foreach (var item in itemsToRemove)
        {
            currentItems.Remove(item);
        }
    }
}