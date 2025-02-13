using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WareHouse.Domain.Models;

namespace WareHouse.Infrastructure.DataBase
{
    internal class RackInceptor:SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            DbContext? context = eventData.Context;

            if (context == null){
                return base.SavingChangesAsync(eventData, result);
            }

            
            foreach(EntityEntry<RackModel> entry in context.ChangeTracker.Entries<RackModel>())
            {
                if (entry.State==EntityState.Added|| entry.State== EntityState.Modified)
                {
                    entry.Entity.TotalPlaces = entry.Entity.RackX * entry.Entity.RackY;
                }
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
