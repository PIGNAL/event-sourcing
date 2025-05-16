using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Ticketing.Query.Domain.Abstractions;

namespace Ticketing.Query.Infrastructure.Persistence.Interceptors
{
    public class AuditEntitiesInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var dbContext = eventData.Context;
            if (dbContext is null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            var entries = dbContext.ChangeTracker.Entries<Entity>();
            foreach (EntityEntry<Entity> entity in entries)
            {
                if (entity.State == EntityState.Added)
                {
                    entity.Property(x=> x.CreatedOn).CurrentValue = DateTime.UtcNow;
                    entity.Property(x => x.CreatedBy).CurrentValue = "ApacheKafka";

                }

                if (entity.State == EntityState.Modified)
                {
                    entity.Property(x=>x.LastModifiedOn).CurrentValue = DateTime.UtcNow;
                    entity.Property(x => x.LastModifiedBy).CurrentValue = "ApacheKafka";
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
