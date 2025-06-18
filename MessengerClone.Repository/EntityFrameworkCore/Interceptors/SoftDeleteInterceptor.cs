using MessengerClone.Domain.Abstractions;
using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MessengerClone.Repository.EntityFrameworkCore.Interceptors
{
    public class SoftDeleteInterceptor(IUserContext _userContext) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            CascadeSoftDelete(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            CascadeSoftDelete(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void CascadeSoftDelete(DbContext? context)
        {
            if (context == null) return;

            var now = DateTime.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                // Soft-delete the entity itself
                if (entry is { State: EntityState.Deleted, Entity: ISoftDeletable sd })
                {
                    entry.State = EntityState.Modified;
                    sd.IsDeleted = true;
                    sd.DateDeleted = now;
                    sd.DeletedBy = _userContext.UserId;
                }
            }

            // Cascade soft delete: Conversation → Members
            // Cascade soft delete: Conversation → Messages
            var convEntries = context.ChangeTracker
                .Entries<Chat>()
                .Where(e => e.Entity.IsDeleted && e.State == EntityState.Modified);

            foreach (var convEntry in convEntries)
            {
                var conv = convEntry.Entity;
                if (conv.Messages != null)
                {
                    foreach (var msg in conv.Messages.Where(m => !m.IsDeleted))
                    {
                        msg.IsDeleted = true;
                        msg.DateDeleted = now;
                        msg.DeletedBy = _userContext.UserId;

                        // Soft-delete MediaAttachment if exists
                        if (msg.Attachment != null && !msg.Attachment.IsDeleted)
                        {
                            msg.Attachment.IsDeleted = true;
                            msg.Attachment.DateDeleted = now;
                            msg.Attachment.DeletedBy = _userContext.UserId;
                        }
                    }
                }
            }

            // Cascade soft delete: Message → MediaAttachment (for standalone deleted messages)
            // Cascade soft delete: Message → Statuses (for standalone deleted messages)
            // Cascade soft delete: Message → Reactions (for standalone deleted messages)
            var messageEntries = context.ChangeTracker
                .Entries<Message>()
                .Where(e => e.Entity.IsDeleted && e.State == EntityState.Modified);

            foreach (var msgEntry in messageEntries)
            {
                var msg = msgEntry.Entity;
                if (msg.Attachment != null && !msg.Attachment.IsDeleted)
                {
                    msg.Attachment.IsDeleted = true;
                    msg.Attachment.DateDeleted = now;
                    msg.Attachment.DeletedBy = _userContext.UserId;
                }
            }
        }
    }
}
