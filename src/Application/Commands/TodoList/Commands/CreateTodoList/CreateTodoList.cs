using Educar.Backend.Application.Interfaces;

namespace Educar.Backend.Application.TodoList.Commands.CreateTodoList;

public record CreateTodoListCommand : IRequest<Guid>
{
    public CreateTodoListCommand(string title)
    {
        Title = title;
    }

    public string Title { get; init; }
}

public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.TodoList(request.Title);

        _context.TodoLists.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}