using System.ComponentModel.DataAnnotations.Schema;
using Educar.Backend.Infrastructure.Data;

namespace Educar.Backend.Domain.Common;

public abstract class BaseEntity: ISoftDelete
{
    private readonly List<BaseEvent> _domainEvents = new();

    // This can easily be modified to be BaseEntity<T> and public T Id to support different key types.
    // Using non-generic integer types for simplicity
    public Guid Id { get; set; }
    
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    [NotMapped] public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}