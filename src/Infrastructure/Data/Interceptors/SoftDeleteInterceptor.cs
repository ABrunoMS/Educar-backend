using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Educar.Backend.Infrastructure.Data.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SoftDeleteEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        SoftDeleteEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SoftDeleteEntities(DbContext? context)
    {
        if (context is null) return;

        var processedEntities = new HashSet<object>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry is { State: EntityState.Deleted, Entity: ISoftDelete delete } &&
                !processedEntities.Contains(entry.Entity))
            {
                SoftDeleteEntity(entry, processedEntities);
            }
        }
    }

    private void SoftDeleteEntity(EntityEntry entry, HashSet<object> processedEntities)
    {
        if (entry.Entity is not ISoftDelete delete) return;

        entry.State = EntityState.Modified;
        delete.IsDeleted = true;
        delete.DeletedAt = DateTimeOffset.UtcNow;

        processedEntities.Add(entry.Entity);

        // Handle child entities
        foreach (var navigationEntry in entry.Navigations)
        {
            switch (navigationEntry)
            {
                case CollectionEntry collectionEntry:
                {
                    if (collectionEntry.CurrentValue != null)
                        foreach (var dependentEntity in collectionEntry.CurrentValue)
                        {
                            if (dependentEntity is not ISoftDelete dependentSoftDelete ||
                                processedEntities.Contains(dependentEntity)) continue;
                            var dependentEntryEntity = entry.Context.Entry(dependentSoftDelete);
                            SoftDeleteEntity(dependentEntryEntity, processedEntities);
                        }

                    break;
                }
                case ReferenceEntry referenceEntry:
                {
                    if (referenceEntry.CurrentValue is ISoftDelete dependentSoftDelete &&
                        !processedEntities.Contains(referenceEntry.CurrentValue))
                    {
                        var dependentEntryEntity = entry.Context.Entry(dependentSoftDelete);
                        SoftDeleteEntity(dependentEntryEntity, processedEntities);
                    }

                    break;
                }
            }
        }
    }
}