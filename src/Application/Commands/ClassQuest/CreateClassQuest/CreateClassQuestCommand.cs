using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Entities;
using System.Globalization;

namespace Educar.Backend.Application.Commands.ClassQuest.CreateClassQuest;

public record CreateClassQuestCommand : IRequest<IdResponseDto>
{
    public Guid ClassId { get; init; }
    public Guid QuestId { get; init; }
    public string StartDate { get; init; } = string.Empty;
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

        // Parse da data de início em formato ISO ou DD/MM/YYYY
        DateTimeOffset startDateOffset;
        
        if (DateTimeOffset.TryParse(request.StartDate, out var parsedStartDateOffset))
        {
            // Formato ISO
            startDateOffset = parsedStartDateOffset;
        }
        else if (DateTime.TryParseExact(
            request.StartDate,
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var startDate))
        {
            // Formato DD/MM/YYYY
            startDateOffset = new DateTimeOffset(startDate, TimeSpan.Zero);
        }
        else
        {
            throw new BadRequestException("Formato de data de início inválido. Use o formato ISO (YYYY-MM-DDTHH:mm:ssZ) ou DD/MM/YYYY.");
        }

        // Parse da data em formato ISO ou DD/MM/YYYY
        DateTimeOffset expirationDateOffset;
        
        if (DateTimeOffset.TryParse(request.ExpirationDate, out var parsedDateOffset))
        {
            // Formato ISO
            expirationDateOffset = parsedDateOffset;
        }
        else if (DateTime.TryParseExact(
            request.ExpirationDate,
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var expirationDate))
        {
            // Formato DD/MM/YYYY
            expirationDateOffset = new DateTimeOffset(expirationDate, TimeSpan.Zero);
        }
        else
        {
            throw new BadRequestException("Formato de data inválido. Use o formato ISO (YYYY-MM-DDTHH:mm:ssZ) ou DD/MM/YYYY.");
        }

        var entity = new Domain.Entities.ClassQuest
        {
            ClassId = request.ClassId,
            QuestId = request.QuestId,
            StartDate = startDateOffset,
            ExpirationDate = expirationDateOffset
        };

        context.ClassQuests.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}
