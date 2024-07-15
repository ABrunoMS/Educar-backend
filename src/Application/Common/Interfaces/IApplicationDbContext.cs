using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Domain.Entities.TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }
    DbSet<Contract> Contracts { get; }
    DbSet<Client> Clients { get; }
    DbSet<Account> Accounts { get; }
    DbSet<Game> Games { get; }
    DbSet<Address> Addresses { get; }
    DbSet<School> Schools { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}