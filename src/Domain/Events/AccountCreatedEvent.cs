using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Domain.Events;

public class AccountCreatedEvent(Account account) : BaseEvent
{
    public Account Account { get; } = account;
}