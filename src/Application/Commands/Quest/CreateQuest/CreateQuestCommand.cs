using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Models;
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
    int TotalQuestSteps,
    CombatDifficulty CombatDifficulty,
    Guid? GameId,
    Guid? GradeId,
    Guid? SubjectId,
    Guid? QuestDependencyId = null,
    IList<Guid>? BnccIds = null // Nome correto da propriedade
) : IRequest<IdResponseDto>;

// O Handler (Lógica)
public class CreateQuestCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateQuestCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateQuestCommand request, CancellationToken cancellationToken)
    {
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

        // 2. Buscar as entidades BNCC
        // Correção: Usando a variável 'bnccEntities' (camelCase) corretamente
        var bnccEntities = await GetBnccsByIdsAsync(context, request.BnccIds, cancellationToken);

        // 3. Criar a Entidade Quest
        var quest = new Domain.Entities.Quest(
            request.Name,
            request.Description,
            request.UsageTemplate,
            request.Type,
            request.MaxPlayers,
            request.TotalQuestSteps,
            request.CombatDifficulty)
        {
            GradeId = request.GradeId,
            SubjectId = request.SubjectId,
            QuestDependency = questDependency
        };

        // 4. Associar BNCCs à Quest
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

        // 5. Salvar
        context.Quests.Add(quest);
        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(quest.Id);
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
            throw new NotFoundException(nameof(Domain.Entities.Bncc), string.Join(", ", missingIds));
        }

        return bnccEntities;
    }
}