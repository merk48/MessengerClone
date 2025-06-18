using MessengerClone.Domain.Abstractions;
using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;

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

            // Cascade soft delete: Chat → Members
            //                           → Messages
            var chatEntries = context.ChangeTracker
                .Entries<Chat>()
                .Where(e => e.Entity.IsDeleted && e.State == EntityState.Modified);

            foreach (var chatEntry in chatEntries)
            {
                var chat = chatEntry.Entity;
                if (chat.Messages != null)
                {
                    foreach (var msg in chat.Messages.Where(m => !m.IsDeleted))
                    {
                        msg.IsDeleted = true;
                        msg.DateDeleted = now;
                        msg.DeletedBy = _userContext.UserId;

                        if (msg.Attachment != null && !msg.Attachment.IsDeleted)
                        {
                            msg.Attachment.IsDeleted = true;
                            msg.Attachment.DateDeleted = now;
                            msg.Attachment.DeletedBy = _userContext.UserId;
                        }

                        if (msg.MessageStatuses != null)
                        {
                            foreach (var status in msg.MessageStatuses.Where(s => !s.IsDeleted))
                            {
                                status.IsDeleted = true;
                                status.DateDeleted = now;
                                status.DeletedBy = _userContext.UserId;
                            }
                        }

                        if (msg.MessageReactions != null)
                        {
                            foreach (var reaction in msg.MessageReactions.Where(r => !r.IsDeleted))
                            {
                                reaction.IsDeleted = true;
                                reaction.DateDeleted = now;
                                reaction.DeletedBy = _userContext.UserId;
                            }
                        }
                    }
                }

                if (chat.ChatMembers != null)
                {
                    foreach (var member in chat.ChatMembers.Where(m => !m.IsDeleted))
                    {
                        member.IsDeleted = true;
                        member.DateDeleted = now;
                        member.DeletedBy = _userContext.UserId;
                    }
                }
            }

            // (for standalone deleted messages)
            // Cascade soft delete: Message → MediaAttachment
            //                              → Statuses 
            //                              → Reactions 
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

                if (msg.MessageStatuses != null)
                {
                    foreach (var status in msg.MessageStatuses.Where(s => !s.IsDeleted))
                    {
                        status.IsDeleted = true;
                        status.DateDeleted = now;
                        status.DeletedBy = _userContext.UserId;
                    }
                }

                if (msg.MessageReactions != null)
                {
                    foreach (var reaction in msg.MessageReactions.Where(r => !r.IsDeleted))
                    {
                        reaction.IsDeleted = true;
                        reaction.DateDeleted = now;
                        reaction.DeletedBy = _userContext.UserId;
                    }
                }
            }

        }
    }
}
