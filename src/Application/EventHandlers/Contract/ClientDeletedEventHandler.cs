using Educar.Backend.Application.Interfaces;
using Educar.Backend.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Educar.Backend.Application.EventHandlers.Client;

public class ClientDeletedEventHandler(IApplicationDbContext context, ILogger<ClientDeletedEventHandler> logger)
    : INotificationHandler<ClientDeletedEvent>
{
    public Task Handle(ClientDeletedEvent notification, CancellationToken cancellationToken)
    {
        var entity = notification.Client;

        if (entity.Contract == null) return Task.CompletedTask;

        context.Contracts.Remove(entity.Contract);
        logger.LogInformation($"Contract {entity.Contract.Id} removed");

        return Task.CompletedTask;
    }
}