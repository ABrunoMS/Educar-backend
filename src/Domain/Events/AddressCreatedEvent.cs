using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Domain.Events;

public class AddressCreatedEvent(Address address) : BaseEvent
{
    public Address Address { get; } = address;
}