using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities; 
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore; 
using MediatR;
using System.Collections.Generic; 
using System.Linq; 
using Ardalis.GuardClauses; 

namespace Educar.Backend.Application.Commands.Class.UpdateClass;


public record UpdateClassCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ClassPurpose? Purpose { get; set; }
    public bool? IsActive { get; set; }
    public string? SchoolYear { get; set; }
    public string? SchoolShift { get; set; }
    public List<Guid>? TeacherIds { get; init; }
    public List<Guid>? StudentIds { get; init; }
    public List<Guid>? ProductIds { get; init; }
    public List<Guid>? ContentIds { get; init; }
    
  
public class UpdateClassCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateClassCommand, Unit>
{
    public async Task<Unit> Handle(UpdateClassCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Classes
            .Include(c => c.AccountClasses)
            .Include(c => c.ClassProducts)   // <-- Adicione Include
            .Include(c => c.ClassContents)  // <-- Adicione Include
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            
        Guard.Against.NotFound(request.Id, entity);

        // Atualização de propriedades simples
        if (request.Name != null) entity.Name = request.Name;
        if (request.Description != null) entity.Description = request.Description;
        if (request.Purpose.HasValue) entity.Purpose = request.Purpose.Value;
        if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        if (request.SchoolYear != null) entity.SchoolYear = request.SchoolYear;
        if (request.SchoolShift != null) entity.SchoolShift = request.SchoolShift;

       
        if (request.TeacherIds != null || request.StudentIds != null)
        {
            var newAccountIds = (request.TeacherIds ?? new List<Guid>())
                .Concat(request.StudentIds ?? new List<Guid>())
                .Distinct().ToList();
            
            UpdateJunctionTable(entity.AccountClasses, newAccountIds, ac => ac.AccountId, id => new AccountClass { AccountId = id });
        }

        if (request.ProductIds != null)
        {
            UpdateJunctionTable(entity.ClassProducts, request.ProductIds, cp => cp.ProductId, id => new ClassProduct { ProductId = id });
        }


        if (request.ContentIds != null)
        {
            UpdateJunctionTable(entity.ClassContents, request.ContentIds, cc => cc.ContentId, id => new ClassContent { ContentId = id });
        }

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    // Método genérico para atualizar tabelas de ligação (Junction Tables)
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
}