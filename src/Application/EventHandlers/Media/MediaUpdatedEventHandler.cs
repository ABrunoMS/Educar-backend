using System.Text.Json;
using Educar.Backend.Application.Common.Extensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Events;

namespace Educar.Backend.Application.EventHandlers.Media;

public class MediaUpdatedEventHandler(IApplicationDbContext context) : INotificationHandler<MediaUpdatedEvent>
{
    public async Task Handle(MediaUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var account = await context.Accounts.FindAsync([notification.AccountId], cancellationToken: cancellationToken);
        Guard.Against.NotFound(notification.AccountId, account);

        var currentState = notification.Media.ToJsonObject();

        var entity = new Domain.Entities.MediaLog(AuditableAction.Update, currentState)
        {
            Account = account,
            Media = notification.Media,
            PreviousState = notification.PreviousState
        };

        context.MediaLogs.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
    }
}