using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Domain.Entities.TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }
    DbSet<Contract> Contracts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}