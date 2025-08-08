using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Domain.Events;

public class AccountDeletedEvent(Account account) : BaseEvent
{
    public Account Account { get; } = account;
}