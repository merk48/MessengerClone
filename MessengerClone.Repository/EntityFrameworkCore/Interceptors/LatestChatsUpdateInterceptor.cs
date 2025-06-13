using MessengerClone.Domain.Abstractions;
using MessengerClone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;


namespace MessengerClone.Repository.EntityFrameworkCore.Interceptors
{
    public class LatestChatsUpdateInterceptor(IUserContext _userContext) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateChat(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            UpdateChat(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateChat(DbContext? context)
        {
            if (context == null) return;

            var now = DateTime.UtcNow;

            // For every newly Added Message, update its Conversation.LastMessageAt
            foreach (var entry in context.ChangeTracker.Entries<Message>().Where(e => e.State == EntityState.Added))
            {
                var msg = entry.Entity;
                // Find the Conversation entry if it's already tracked
                var chatEntry = context.ChangeTracker.Entries<Chat>()
                                       .FirstOrDefault(e => e.Entity.Id == msg.ChatId);

                if (chatEntry != null)
                {
                    chatEntry.Entity.LastMessage = new LastMessageSnapshot
                    {
                        Id = msg.Id,
                        Content = msg.Content,
                        SentAt = msg.CreatedAt,
                        SenderUserame = msg.Sender.UserName!,
                        Type = msg.Type
                    };

                }
                else
                {
                    // Otherwise, load a stub Chat and set the property
                    var chat = new Chat { Id = msg.ChatId };
                    context.Attach(chat);
                    chat.LastMessage = new LastMessageSnapshot
                    {
                        Id = msg.Id,
                        Content = msg.Content,
                        SentAt = msg.CreatedAt,
                        SenderUserame = _userContext.UserName!,
                        Type = msg.Type
                    };
                }
            }
        }
    }


}
