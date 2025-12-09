using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Common; // Para NotFoundException
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Application.Commands.Quest.UpdateQuest;

// O Comando (Request)
public record UpdateQuestCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? UsageTemplate { get; set; }
    public QuestType? Type { get; set; }
    public int? MaxPlayers { get; set; }
    public int? TotalQuestSteps { get; set; }
    public CombatDifficulty? CombatDifficulty { get; set; }
    
    // IDs de relacionamento
    public Guid? GradeId { get; set; }
    public Guid? SubjectId { get; set; }
    public Guid? QuestDependencyId { get; set; }
    
    
    public IList<Guid>? BnccIds { get; set; } 
}


public class UpdateQuestCommandHandler : IRequestHandler<UpdateQuestCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateQuestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateQuestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Quests
           
            .Include(q => q.BnccQuests) 
            .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);
            
        Guard.Against.NotFound(request.Id, entity);

        UpdateQuestProperties(entity, request);

        
        await UpdateBnccRelationships(_context, entity, request.BnccIds, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private void UpdateQuestProperties(Domain.Entities.Quest entity, UpdateQuestCommand request)
    {
        if (request.Name != null) entity.Name = request.Name;
        if (request.Description != null) entity.Description = request.Description;
        if (request.UsageTemplate.HasValue) entity.UsageTemplate = request.UsageTemplate.Value;
        if (request.Type.HasValue) entity.Type = request.Type.Value;
        if (request.MaxPlayers.HasValue) entity.MaxPlayers = request.MaxPlayers.Value;
        if (request.TotalQuestSteps.HasValue) entity.TotalQuestSteps = request.TotalQuestSteps.Value;
        if (request.CombatDifficulty.HasValue) entity.CombatDifficulty = request.CombatDifficulty.Value;
        if (request.GradeId.HasValue) entity.GradeId = request.GradeId.Value;
        if (request.SubjectId.HasValue) entity.SubjectId = request.SubjectId.Value;

        if (request.QuestDependencyId.HasValue)
        {
          
             var dep = _context.Quests.Find(request.QuestDependencyId.Value);
             if(dep != null) entity.QuestDependency = dep;
        }
    }

   
   private async Task UpdateBnccRelationships(
    IApplicationDbContext context, 
    Domain.Entities.Quest entity,
    IList<Guid>? bnccIds, 
    CancellationToken cancellationToken)
{
    // Se for null, não faz nada (mantém o que está). 
    if (bnccIds == null) return;

    // 1. Validar se os IDs enviados existem na tabela mestre de BNCCs
    
    var existingBnccs = await context.Bnccs
        .AsNoTracking() 
        .Where(b => bnccIds.Contains(b.Id) && !b.IsDeleted)
        .Select(b => b.Id) // Trazemos apenas os IDs para comparar
        .ToListAsync(cancellationToken);

    var missingIds = bnccIds.Except(existingBnccs).ToList();
    if (missingIds.Any())
    {
        throw new NotFoundException(nameof(Domain.Entities.Bncc), string.Join(", ", missingIds));
    }

    
    var currentRelationships = entity.BnccQuests.ToList(); 

    // IDs que estão atualmente vinculados (ativos ou inativos)
    var currentBnccIds = currentRelationships.Select(r => r.BnccId).ToList();

    // 3. Calcular Delta
    var idsToAdd = bnccIds.Except(currentBnccIds).ToList();
    
   
    var idsToRemove = currentRelationships
        .Where(x => !x.IsDeleted && !bnccIds.Contains(x.BnccId))
        .Select(x => x.BnccId)
        .ToList();

    // 4. Adicionar ou Reativar
    foreach (var id in idsToAdd)
    {
        // Verifica se já existe o relacionamento (mesmo que deletado) na memória
        var existingRel = currentRelationships.FirstOrDefault(r => r.BnccId == id);
        
        if (existingRel != null)
        {
            
            if (existingRel.IsDeleted)
            {
                existingRel.IsDeleted = false;
                existingRel.DeletedAt = null;
                
            }
        }
        else
        {
            
            entity.BnccQuests.Add(new BnccQuest 
            { 
                QuestId = entity.Id, 
                BnccId = id 
                
            });
        }
    }

    // 5. Remover (Soft Delete)
    foreach (var id in idsToRemove)
    {
        var rel = currentRelationships.First(r => r.BnccId == id);
        rel.IsDeleted = true;
        rel.DeletedAt = DateTimeOffset.UtcNow; 
    }
}
}