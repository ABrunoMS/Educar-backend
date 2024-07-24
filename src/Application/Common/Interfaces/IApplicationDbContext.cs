using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Contract> Contracts { get; }
    DbSet<Client> Clients { get; }
    DbSet<Account> Accounts { get; }
    DbSet<Game> Games { get; }
    DbSet<Address> Addresses { get; }
    DbSet<School> Schools { get; }
    DbSet<Media> Medias { get; }
    DbSet<MediaLog> MediaLogs { get; }
    DbSet<Class> Classes { get; }
    DbSet<AccountClass> AccountClasses { get; }
    DbSet<Grade> Grades { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}