using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Domain.Events;

public class ClientDeletedEvent(Client client) : BaseEvent
{
    public Client Client { get; } = client;
}