using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Educar.Backend.Application.EventHandlers.Account;

public class AccountDeletedEventHandler(
    ILogger<AccountDeletedEventHandler> logger,
    IIdentityService identityService)
    : INotificationHandler<AccountDeletedEvent>
{
    public async Task Handle(AccountDeletedEvent notification, CancellationToken cancellationToken)
    {
        var success = await identityService.DeleteUser(notification.Account.Id, cancellationToken);

        if (!success) throw new Exception("Failed to delete keycloak user");

        logger.LogInformation("Keycloak user with id {AccountId} deleted successfully", notification.Account.Id);
    }
}