using Educar.Backend.Application.Common.Interfaces;
using MediatR;
using System.Collections.Generic;
using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Ardalis.GuardClauses;
using System.Linq;

namespace Educar.Backend.Application.Commands.Client.UpdateClient;

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
    public string? Secretary { get; init; }
    public List<Guid>? ProductIds { get; init; }
    public List<Guid>? ContentIds { get; init; }
    public string? SubSecretary { get; set; }
    public string? Regional { get; set; }
}

public class UpdateClientCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateClientCommand, Unit>
{
    public async Task<Unit> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        // 1. Busca a entidade principal E suas listas de ligação atuais
        var entity = await context.Clients
            .Include(c => c.ClientProducts)
            .Include(c => c.ClientContents)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        // 2. Atualiza as propriedades simples (usando ?? para manter o valor antigo se o novo for nulo)
        entity.Name = request.Name ?? entity.Name;
        entity.Description = request.Description ?? entity.Description;
        entity.Partner = request.Partner ?? entity.Partner;
        entity.Contacts = request.Contacts ?? entity.Contacts;
        entity.Contract = request.Contract ?? entity.Contract;
        entity.Validity = request.Validity ?? entity.Validity;
        entity.SignatureDate = request.SignatureDate ?? entity.SignatureDate;
        entity.ImplantationDate = request.ImplantationDate ?? entity.ImplantationDate;
        entity.TotalAccounts = request.TotalAccounts ?? entity.TotalAccounts;
        entity.Secretary = request.Secretary ?? entity.Secretary;
        entity.SubSecretary = request.SubSecretary ?? entity.SubSecretary;
        entity.Regional = request.Regional ?? entity.Regional;

        // 3. Atualiza as tabelas de ligação (Products)
        if (request.ProductIds != null)
        {
            UpdateJunctionTable(entity.ClientProducts, request.ProductIds, cp => cp.ProductId, id => new ClientProduct { ProductId = id });
        }

        // 4. Atualiza as tabelas de ligação (Contents)
        if (request.ContentIds != null)
        {
            UpdateJunctionTable(entity.ClientContents, request.ContentIds, cc => cc.ContentId, id => new ClientContent { ContentId = id });
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
        
        // 1. Adicionar novos
        var idsToAdd = newItemIds.Except(currentIds).ToList();
        foreach (var id in idsToAdd)
        {
            currentItems.Add(createFactory(id));
        }

        // 2. Remover antigos
        var idsToRemove = currentIds.Except(newItemIds).ToList();
        var itemsToRemove = currentItems.Where(item => idsToRemove.Contains(keySelector(item))).ToList();
        foreach (var item in itemsToRemove)
        {
            currentItems.Remove(item);
        }
    }
}