using System.Text.Json;
using System.Text.Json.Nodes;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Events;

namespace Educar.Backend.Application.Commands.MediaLog.CreateMediaLog;

public record CreateMediaLogCommand(AuditableAction Action, JsonObject CurrentState, Guid AccountId, Guid MediaId)
    : IRequest<IdResponseDto>
{
    public JsonObject? PreviousState { get; set; }
}

public class CreateMediaLogCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateMediaLogCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateMediaLogCommand request, CancellationToken cancellationToken)
    {
        var account = await context.Accounts.FindAsync([request.AccountId], cancellationToken: cancellationToken);
        if (account == null) throw new NotFoundException(nameof(account), request.AccountId.ToString());

        var media = await context.Medias.FindAsync([request.MediaId], cancellationToken: cancellationToken);
        if (media == null) throw new NotFoundException(nameof(media), request.MediaId.ToString());

        var entity = new Domain.Entities.MediaLog(request.Action, request.CurrentState)
        {
            Account = account,
            Media = media,
            PreviousState = request.PreviousState
        };

        context.MediaLogs.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}