using MessemgerClone.Domain.Entities.Identity;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MessengerClone.Repository.EntityFrameworkCore.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        protected AppDbContext() : base() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
       

       public DbSet<ApplicationUser> ApplicationUsers { get; set; }
       public DbSet<Chat> Conversations { get; set; }
       public DbSet<DirectChat> DirectConversations { get; set; }
       public DbSet<GroupChat> GroupConversations { get; set; }
       public DbSet<ChatMember> ConversationParticipants { get; set; }
       public DbSet<Message> Messages { get; set; }
       public DbSet<MessageReaction> MessageReactions { get; set; }
       public DbSet<MessageStatus> MessageReadReceipts { get; set; }
       public DbSet<MediaAttachment> MediaAttachments { get; set; }
       public DbSet<UserLog> UserLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
