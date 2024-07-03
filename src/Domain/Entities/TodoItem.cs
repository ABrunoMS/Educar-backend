using Educar.Backend.Domain.Events;

namespace Educar.Backend.Domain.Entities;

public class TodoItem : BaseAuditableEntity
{
    private bool _done;
    public string? Title { get; set; }

    public string? Note { get; set; }

    public DateTime? Reminder { get; set; }

    public bool Done
    {
        get => _done;
        set
        {
            if (value && !_done) AddDomainEvent(new TodoItemCompletedEvent(this));

            _done = value;
        }
    }

    public TodoList List { get; set; } = null!;
}