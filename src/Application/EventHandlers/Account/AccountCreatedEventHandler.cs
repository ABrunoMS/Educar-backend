using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Educar.Backend.Application.EventHandlers.Account;

public class AccountCreatedEventHandler(
    ILogger<AccountCreatedEventHandler> logger,
    IIdentityService identityService)
    : INotificationHandler<AccountCreatedEvent>
{
    public async Task Handle(AccountCreatedEvent notification, CancellationToken cancellationToken)
    {
        var userId = await identityService.CreateUser(notification.Account.Email, notification.Account.Name,
            notification.Account.Role, cancellationToken);

        if (userId == Guid.Empty) throw new Exception("Failed to create user");

        //Saving isn't required here because this entity is managed and hasn't been saved yet at this point
        notification.Account.Id = userId;

        //TODO send email with new password?

        logger.LogInformation("Keycloak user with id {AccountId} created successfully", notification.Account.Id);
    }
}