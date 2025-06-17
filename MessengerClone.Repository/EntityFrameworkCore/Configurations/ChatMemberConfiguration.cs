using MessengerClone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessengerClone.Repository.EntityFrameworkCore.Configurations
{
    public class ChatMemberConfiguration : IEntityTypeConfiguration<ChatMember>
    {
        public void Configure(EntityTypeBuilder<ChatMember> builder)
        {
            builder.HasKey(cp => new { cp.UserId, cp.ChatId });

            builder.Property(x => x.ChatRole)
                   .HasConversion<string>();

            builder.Property(cp => cp.CreatedAt)
             .IsRequired();

            builder.HasOne(cp => cp.User)
                .WithMany(u => u.ChatMembers)
                .HasForeignKey(cp => cp.UserId)
             .OnDelete(DeleteBehavior.Cascade);
           
            builder.HasOne(cp => cp.Chat)
              .WithMany(c => c.ChatMembers)
              .HasForeignKey(cp => cp.ChatId)
             .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("ChatMembers");
        }
    }


}

