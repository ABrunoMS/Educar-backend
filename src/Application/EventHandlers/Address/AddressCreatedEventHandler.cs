using Educar.Backend.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Educar.Backend.Application.EventHandlers.Address;

public class AddressCreatedEventHandler(ILogger<AddressCreatedEventHandler> logger)
    : INotificationHandler<AddressCreatedEvent>
{
    public Task Handle(AddressCreatedEvent notification, CancellationToken cancellationToken)
    {
        //TODO use GoogleMaps api to populate lat and lng if not provided

        logger.LogInformation("Address created successfully");

        return Task.CompletedTask;
    }
}