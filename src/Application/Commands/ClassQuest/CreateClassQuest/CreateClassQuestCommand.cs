using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Entities;
using System.Globalization;

namespace Educar.Backend.Application.Commands.ClassQuest.CreateClassQuest;

public record CreateClassQuestCommand : IRequest<IdResponseDto>
{
    public Guid ClassId { get; init; }
    public Guid QuestId { get; init; }
    public string ExpirationDate { get; init; } = string.Empty;
}

public class CreateClassQuestCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateClassQuestCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateClassQuestCommand request, CancellationToken cancellationToken)
    {
        // Validar se a classe existe
        var classEntity = await context.Classes.FindAsync([request.ClassId], cancellationToken: cancellationToken);
        if (classEntity == null)
            throw new Application.Common.Exceptions.NotFoundException(nameof(Class), request.ClassId.ToString());

        // Validar se a quest existe
        var quest = await context.Quests.FindAsync([request.QuestId], cancellationToken: cancellationToken);
        if (quest == null)
            throw new Application.Common.Exceptions.NotFoundException(nameof(Quest), request.QuestId.ToString());

        // Verificar se o relacionamento já existe
        var existingClassQuest = await context.ClassQuests
            .FirstOrDefaultAsync(cq => cq.ClassId == request.ClassId && cq.QuestId == request.QuestId, cancellationToken);
        
        if (existingClassQuest != null)
            throw new BadRequestException("Esta quest já está associada a esta turma.");

        // Parse da data no formato DD/MM/YYYY
        if (!DateTime.TryParseExact(
            request.ExpirationDate,
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var expirationDate))
        {
            throw new BadRequestException("Formato de data inválido. Use o formato DD/MM/YYYY.");
        }

        // Converter para UTC para PostgreSQL
        var expirationDateUtc = DateTime.SpecifyKind(expirationDate, DateTimeKind.Utc);

        var entity = new Domain.Entities.ClassQuest
        {
            ClassId = request.ClassId,
            QuestId = request.QuestId,
            ExpirationDate = expirationDateUtc
        };

        context.ClassQuests.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}
