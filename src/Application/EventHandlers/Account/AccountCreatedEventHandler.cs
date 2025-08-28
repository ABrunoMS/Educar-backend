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
        var password = notification.Account.Password;
            if (string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException("The password was not provided for the creation of the user in Keycloak.");
            }
        
        var lastName = notification.Account.LastName;
        if (string.IsNullOrEmpty(lastName))
        {
            throw new InvalidOperationException("The last name was not provided for the creation of the user in Keycloak.");
        }

        var userId = await identityService.CreateUser(
            notification.Account.Email, 
            notification.Account.Name,
            lastName,
            password,
            notification.Account.Role, 
            cancellationToken);

        if (userId == Guid.Empty) throw new Exception("Failed to create user");


        //Saving isn't required here because this entity is managed and hasn't been saved yet at this point
        notification.Account.Id = userId;

        //TODO send email with new password?

        logger.LogInformation("Keycloak user with id {AccountId} created successfully", notification.Account.Id);
       // return Task.CompletedTask;
    }
}