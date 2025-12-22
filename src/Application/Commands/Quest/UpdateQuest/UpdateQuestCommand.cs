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
    public Guid? ContentId { get; set; }
    public Guid? ProductId { get; set; }
    
    public IList<Guid>? BnccIds { get; set; } 
}


public class UpdateQuestCommandHandler : IRequestHandler<UpdateQuestCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public UpdateQuestCommandHandler(IApplicationDbContext context, IUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(UpdateQuestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Quests
           
            .Include(q => q.BnccQuests) 
            .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);
            
        Guard.Against.NotFound(request.Id, entity);

        // Validar se o usuário pode atualizar para aula template
        ValidateTemplatePermission(request.UsageTemplate, entity.UsageTemplate);

        // Validar Content e Product se foram informados
        await ValidateClientOwnsContentAndProduct(_context, _currentUser, request.ContentId, request.ProductId, cancellationToken);

        UpdateQuestProperties(entity, request);

        
        await UpdateBnccRelationships(_context, entity, request.BnccIds, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    // Método para validar se o usuário pode criar/atualizar aulas template
    private void ValidateTemplatePermission(bool? requestUsageTemplate, bool currentUsageTemplate)
    {
        // Se não está alterando UsageTemplate para true, não precisa validar
        // Verifica se está tentando criar template (novo ou alterando para true)
        var isCreatingTemplate = requestUsageTemplate == true || (requestUsageTemplate == null && currentUsageTemplate);
        
        if (!isCreatingTemplate) return;

        var userRoles = _currentUser.Roles ?? new List<string>();
        var adminRoleName = UserRole.Admin.ToString();
        var teacherEducarRoleName = UserRole.TeacherEducar.ToString();

        // Apenas Admin e TeacherEducar podem criar/manter aulas template
        if (!userRoles.Contains(adminRoleName) && !userRoles.Contains(teacherEducarRoleName))
        {
            var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("UsageTemplate", "Apenas usuários com cargo Admin ou Professor Educar podem criar ou manter aulas template.")
            };
            throw new Educar.Backend.Application.Common.Exceptions.ValidationException(failures);
        }
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
        if (request.ContentId.HasValue) entity.ContentId = request.ContentId.Value;
        if (request.ProductId.HasValue) entity.ProductId = request.ProductId.Value;

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

    // Método para validar se o cliente possui o Content e Product especificados
    // e se o Content pertence ao Product
    private async Task ValidateClientOwnsContentAndProduct(
        IApplicationDbContext context,
        IUser currentUser,
        Guid? contentId,
        Guid? productId,
        CancellationToken cancellationToken)
    {
        // Se não informou nem Content nem Product, não há validação a fazer
        if (!contentId.HasValue && !productId.HasValue)
            return;

        // Se apenas um foi informado, exige que ambos sejam informados
        if (!contentId.HasValue || !productId.HasValue)
        {
            var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("ContentId/ProductId", "ContentId e ProductId devem ser informados juntos.")
            };
            throw new Educar.Backend.Application.Common.Exceptions.ValidationException(failures);
        }

        // 1. Validar se o Content pertence ao Product
        var productHasContent = await context.ProductContents
            .AsNoTracking()
            .AnyAsync(pc => pc.ProductId == productId.Value && pc.ContentId == contentId.Value, cancellationToken);

        if (!productHasContent)
        {
            var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("ContentId", "O conteúdo especificado não pertence ao produto informado.")
            };
            throw new Educar.Backend.Application.Common.Exceptions.ValidationException(failures);
        }

        // 2. Obter o ClientId do usuário atual através da conta (Account)
        var userId = currentUser.Id;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Usuário não autenticado.");

        var account = await context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id.ToString() == userId, cancellationToken);

        if (account?.ClientId == null)
            throw new UnauthorizedAccessException("Usuário não está associado a um cliente.");

        var clientId = account.ClientId.Value;

        // 3. Validar se o cliente possui o Content
        var clientHasContent = await context.ClientContents
            .AsNoTracking()
            .AnyAsync(cc => cc.ClientId == clientId && cc.ContentId == contentId.Value, cancellationToken);

        if (!clientHasContent)
        {
            var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("ContentId", "O cliente não possui acesso ao conteúdo especificado.")
            };
            throw new Educar.Backend.Application.Common.Exceptions.ValidationException(failures);
        }

        // 4. Validar se o cliente possui o Product
        var clientHasProduct = await context.ClientProducts
            .AsNoTracking()
            .AnyAsync(cp => cp.ClientId == clientId && cp.ProductId == productId.Value, cancellationToken);

        if (!clientHasProduct)
        {
            var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("ProductId", "O cliente não possui acesso ao produto especificado.")
            };
            throw new Educar.Backend.Application.Common.Exceptions.ValidationException(failures);
        }
    }
}
