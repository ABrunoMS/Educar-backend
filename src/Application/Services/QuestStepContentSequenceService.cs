using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Application.Services;

public interface IQuestStepContentSequenceService
{
    Task ReorderSequencesAsync(Guid questStepId, int newSequence, Guid? excludeContentId, CancellationToken cancellationToken);
    Task<int> GetNextSequenceAsync(Guid questStepId, CancellationToken cancellationToken);
}

public class QuestStepContentSequenceService : IQuestStepContentSequenceService
{
    private readonly IApplicationDbContext _context;

    public QuestStepContentSequenceService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Reorganiza as sequências para evitar duplicatas e buracos
    /// </summary>
    /// <param name="questStepId">ID da etapa</param>
    /// <param name="newSequence">Nova sequência sendo inserida/atualizada</param>
    /// <param name="excludeContentId">ID do conteúdo a excluir (no caso de update)</param>
    public async Task ReorderSequencesAsync(Guid questStepId, int newSequence, Guid? excludeContentId, CancellationToken cancellationToken)
    {
        // Buscar todos os conteúdos da etapa, excluindo o que está sendo atualizado
        var query = _context.QuestStepContents
            .Where(qsc => qsc.QuestStepId == questStepId);

        if (excludeContentId.HasValue)
        {
            query = query.Where(qsc => qsc.Id != excludeContentId.Value);
        }

        var existingContents = await query
            .OrderBy(qsc => qsc.Sequence)
            .ToListAsync(cancellationToken);

        // Reorganizar sequências
        var currentSequence = 1;
        foreach (var content in existingContents)
        {
            // Se chegamos na posição onde o novo conteúdo será inserido, pula
            if (currentSequence == newSequence)
            {
                currentSequence++;
            }

            content.Sequence = currentSequence;
            currentSequence++;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Retorna a próxima sequência disponível (último + 1)
    /// </summary>
    public async Task<int> GetNextSequenceAsync(Guid questStepId, CancellationToken cancellationToken)
    {
        var maxSequence = await _context.QuestStepContents
            .Where(qsc => qsc.QuestStepId == questStepId)
            .MaxAsync(qsc => (int?)qsc.Sequence, cancellationToken);

        return (maxSequence ?? 0) + 1;
    }
}
