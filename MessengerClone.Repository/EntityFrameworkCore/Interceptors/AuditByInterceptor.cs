using MessengerClone.Domain.Abstractions;
using MessengerClone.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;


namespace MessengerClone.Repository.EntityFrameworkCore.Interceptors
{
    public class AuditByInterceptor(IUserContext _userContext) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplyAuditBy(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ApplyAuditBy(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplyAuditBy(DbContext? context)
        {
            if (context == null) return;

            int? userId = null;
            try
            {
                userId = _userContext.UserId;
            }
            catch
            {
                // No HttpContext or UserId available
            }

            if (userId == null) return; // skip audit if no user

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.Entity is ICreatedBy cb && entry.State == EntityState.Added)
                    cb.CreatedBy = userId.Value;
                if (entry.Entity is IUpdatedBy ub && entry.State == EntityState.Modified)
                    ub.UpdatedBy = userId.Value;
            }
        }
    }


}
