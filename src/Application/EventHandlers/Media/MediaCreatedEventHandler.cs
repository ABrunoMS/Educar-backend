using Educar.Backend.Application.Common.Extensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Events;

namespace Educar.Backend.Application.EventHandlers.Media;

public class MediaCreatedEventHandler(IApplicationDbContext context) : INotificationHandler<MediaCreatedEvent>
{
    public async Task Handle(MediaCreatedEvent notification, CancellationToken cancellationToken)
    {
        var account = await context.Accounts.FindAsync([notification.AccountId], cancellationToken: cancellationToken);
        Guard.Against.NotFound(notification.AccountId, account);

        var currentState = notification.Media.ToJsonObject();

        var entity = new Domain.Entities.MediaLog(AuditableAction.Create, currentState)
        {
            Account = account,
            Media = notification.Media
        };

        context.MediaLogs.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
    }
}