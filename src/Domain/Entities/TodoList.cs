namespace Educar.Backend.Domain.Entities;

public class TodoList : BaseAuditableEntity
{
    public TodoList(string title)
    {
        Title = title;
    }

    public string Title { get; set; }

    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
}