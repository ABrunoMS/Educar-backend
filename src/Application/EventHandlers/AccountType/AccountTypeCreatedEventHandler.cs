using Educar.Backend.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Educar.Backend.Application.EventHandlers.AccountType;

// public class AccountTypeCreatedEventHandler(
//     ILogger<AccountTypeCreatedEventHandler> logger,
//     IIdentityService identityService)
//     : INotificationHandler<ClientCreatedEvent>
// {
//     private readonly ILogger<AccountTypeCreatedEventHandler> _logger = logger;
//
//     private readonly IIdentityService _identityService = identityService;
//
//     public async Task Handle(ClientCreatedEvent notification, CancellationToken cancellationToken)
//     {
//         var entity = notification.Client;
//         var roleCreated = await _identityService.CreateRole(entity.Name, cancellationToken);
//
//         if (roleCreated)
//             _logger.LogInformation("Role {RoleName} created", entity.Name);
//         else
//             _logger.LogError("Failed to create role {RoleName}", entity.Name);
//     }
// }