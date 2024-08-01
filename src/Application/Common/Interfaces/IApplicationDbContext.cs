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
    DbSet<Subject> Subjects { get; }
    DbSet<GameSubject> GameSubjects { get; }
    DbSet<Proficiency> Proficiencies { get; }
    DbSet<ProficiencyGroup> ProficiencyGroups { get; }
    DbSet<ProficiencyGroupProficiency> ProficiencyGroupProficiencies { get; }
    DbSet<GameProficiencyGroup> GameProficiencyGroups { get; }
    DbSet<Item> Items { get; }
    DbSet<Npc> Npcs { get; }
    DbSet<NpcItem> NpcItems { get; }
    DbSet<Dialogue> Dialogues { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}