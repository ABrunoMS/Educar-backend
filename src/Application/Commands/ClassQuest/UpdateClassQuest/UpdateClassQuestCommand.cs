using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Exceptions;
using System.Globalization;

namespace Educar.Backend.Application.Commands.ClassQuest.UpdateClassQuest;

public record UpdateClassQuestCommand(Guid Id, string ExpirationDate) : IRequest;

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

        entity.ExpirationDate = expirationDateOffset;

        await context.SaveChangesAsync(cancellationToken);
    }
}
