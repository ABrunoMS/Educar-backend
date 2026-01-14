using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Exceptions;
using System.Globalization;

namespace Educar.Backend.Application.Commands.ClassQuest.UpdateClassQuest;

public record UpdateClassQuestCommand(Guid Id, string? StartDate, string ExpirationDate) : IRequest;

public class UpdateClassQuestCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateClassQuestCommand>
{
    public async Task Handle(UpdateClassQuestCommand request, CancellationToken cancellationToken)
    {
        // Buscar o relacionamento existente pelo ID
        var entity = await context.ClassQuests
            .FirstOrDefaultAsync(cq => cq.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new Application.Common.Exceptions.NotFoundException(nameof(Domain.Entities.ClassQuest), request.Id.ToString());

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

        // Atualizar StartDate se fornecido
        if (!string.IsNullOrEmpty(request.StartDate))
        {
            DateTimeOffset startDateOffset;
            
            if (DateTimeOffset.TryParse(request.StartDate, out var parsedStartDateOffset))
            {
                startDateOffset = parsedStartDateOffset;
            }
            else if (DateTime.TryParseExact(
                request.StartDate,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var startDate))
            {
                startDateOffset = new DateTimeOffset(startDate, TimeSpan.Zero);
            }
            else
            {
                throw new BadRequestException("Formato de data de início inválido. Use o formato ISO (YYYY-MM-DDTHH:mm:ssZ) ou DD/MM/YYYY.");
            }
            
            entity.StartDate = startDateOffset;
        }

        entity.ExpirationDate = expirationDateOffset;

        await context.SaveChangesAsync(cancellationToken);
    }
}
