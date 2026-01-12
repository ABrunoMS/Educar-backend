using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Common; // Para NotFoundException
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Application.Commands.Quest.CreateQuest;

// O Comando (Request)
public record CreateQuestCommand(
    string Name,
    string Description,
    bool UsageTemplate,
    QuestType Type,
    int MaxPlayers,
    CombatDifficulty CombatDifficulty,
    Guid ContentId,
    Guid ProductId,
    Guid? GameId,
    Guid? GradeId,
    Guid? SubjectId,
    Guid? QuestDependencyId = null,
    IList<Guid>? BnccIds = null
) : IRequest<IdResponseDto>;

// O Handler (Lógica)
public class CreateQuestCommandHandler(IApplicationDbContext context, IUser currentUser)
    : IRequestHandler<CreateQuestCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateQuestCommand request, CancellationToken cancellationToken)
    {
        // 0. Validar se o usuário pode criar aulas template
        ValidateTemplatePermission(request.UsageTemplate);

        // 1. Validação de Dependência de Quest (Mantida)
        Domain.Entities.Quest? questDependency = null;
        if (request.QuestDependencyId.HasValue)
        {
            questDependency = await GetEntityByIdAsync<Domain.Entities.Quest>(
                context.Quests,
                request.QuestDependencyId.Value, 
                nameof(Domain.Entities.Quest), 
                cancellationToken);
        }

        // 2. Validar se o cliente do usuário possui o Content e Product especificados
        await ValidateClientOwnsContentAndProduct(context, currentUser, request.ContentId, request.ProductId, cancellationToken);

        // 3. Buscar as entidades BNCC
        // Correção: Usando a variável 'bnccEntities' (camelCase) corretamente
        var bnccEntities = await GetBnccsByIdsAsync(context, request.BnccIds, cancellationToken);

        // 4. Criar a Entidade Quest
        var quest = new Domain.Entities.Quest(
            request.Name,
            request.Description,
            request.UsageTemplate,
            request.Type,
            request.MaxPlayers,
            null,
            request.CombatDifficulty)
        {
            GradeId = request.GradeId,
            SubjectId = request.SubjectId,
            QuestDependency = questDependency,
            ContentId = request.ContentId,
            ProductId = request.ProductId
        };

        // 5. Associar BNCCs à Quest
        // Correção: Iterando sobre a variável correta 'bnccEntities'
        foreach (var bnccEntity in bnccEntities)
        {
            // Adiciona na tabela de junção BnccQuests
            quest.BnccQuests.Add(new BnccQuest 
            { 
                Quest = quest, 
                Bncc = bnccEntity 
            });
        }

        // 6. Salvar
        context.Quests.Add(quest);
        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(quest.Id);
    }

    // Método para validar se o usuário pode criar aulas template
    private void ValidateTemplatePermission(bool usageTemplate)
    {
        // Se não está criando template, não precisa validar
        if (!usageTemplate) return;

        var userRoles = currentUser.Roles ?? new List<string>();
        var adminRoleName = UserRole.Admin.ToString();
        var teacherEducarRoleName = UserRole.TeacherEducar.ToString();

        // Apenas Admin e TeacherEducar podem criar aulas template
        if (!userRoles.Contains(adminRoleName) && !userRoles.Contains(teacherEducarRoleName))
        {
            var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("UsageTemplate", "Apenas usuários com cargo Admin ou Professor Educar podem criar aulas template.")
            };
            throw new Educar.Backend.Application.Common.Exceptions.ValidationException(failures);
        }
    }

    // Método Auxiliar Genérico (Mantido)
    private async Task<TEntity> GetEntityByIdAsync<TEntity>(DbSet<TEntity> dbSet, Guid id, string entityName,
        CancellationToken cancellationToken) where TEntity : BaseAuditableEntity
    {
        var entity = await dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        Guard.Against.NotFound(id, entity);
        return entity;
    }

    // Método Auxiliar Específico para BNCC (Corrigido)
    private async Task<List<Domain.Entities.Bncc>> GetBnccsByIdsAsync(
        IApplicationDbContext context,
        IList<Guid>? bnccIds, // Nome do parâmetro corrigido
        CancellationToken cancellationToken)
    {
        // Verifica se a lista é nula ou vazia
        if (bnccIds is not { Count: > 0 }) return new List<Domain.Entities.Bncc>();

        // Busca no banco
        var bnccEntities = await context.Bnccs
            .Where(p => bnccIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        // Verifica se algum ID não foi encontrado
        var missingIds = bnccIds.Except(bnccEntities.Select(p => p.Id)).ToList();
        
        // Correção do erro CS0019: missingIds é uma List, então .Count é propriedade (sem parênteses)
        if (missingIds.Count != 0)
        {
            throw new Educar.Backend.Application.Common.Exceptions.NotFoundException(nameof(Domain.Entities.Bncc), string.Join(", ", missingIds));
        }

        return bnccEntities;
    }

    // Método para validar se o cliente possui o Content e Product especificados
    // e se o Content pertence ao Product
    private async Task ValidateClientOwnsContentAndProduct(
        IApplicationDbContext context,
        IUser currentUser,
        Guid contentId,
        Guid productId,
        CancellationToken cancellationToken)
    {
        // 1. Validar se o Content pertence ao Product
        var productHasContent = await context.ProductContents
            .AsNoTracking()
            .AnyAsync(pc => pc.ProductId == productId && pc.ContentId == contentId, cancellationToken);

        if (!productHasContent)
        {
            var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("ContentId", "O conteúdo especificado não pertence ao produto informado.")
            };
            throw new Educar.Backend.Application.Common.Exceptions.ValidationException(failures);
        }

        var userRoles = currentUser.Roles ?? new List<string>();
        var adminRole = UserRole.Admin.ToString();
        var teacherEducarRole = UserRole.TeacherEducar.ToString();

        if (userRoles.Contains(adminRole) || userRoles.Contains(teacherEducarRole))
        {
            return; // Libera o acesso sem checar vínculo com cliente
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
            .AnyAsync(cc => cc.ClientId == clientId && cc.ContentId == contentId, cancellationToken);

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
            .AnyAsync(cp => cp.ClientId == clientId && cp.ProductId == productId, cancellationToken);

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
