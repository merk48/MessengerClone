using MessengerClone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessengerClone.Repository.EntityFrameworkCore.Configurations
{
    public class MessageReactionConfiguration : IEntityTypeConfiguration<MessageReaction>
    {
        public void Configure(EntityTypeBuilder<MessageReaction> builder)
        {
            builder.HasKey(mr => new { mr.MessageId, mr.UserId, mr.ChatId});

            builder.Property(mr => mr.ReactionType)
                .HasConversion<string>() //enMessageReactionType
                .IsRequired();

            builder.Property(mr => mr.CreatedAt)
             .IsRequired();

            builder.Property(c => c.DateDeleted)
             .IsRequired(false);

            builder
                .HasOne(mr => mr.Message)
                .WithMany(m => m.MessageReactions)
                .HasForeignKey(mr => mr.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder
                .HasOne(mr => mr.Member)
                .WithMany(cm => cm.MessageReactions)
                .HasForeignKey(x => new { x.UserId, x.ChatId })
                .HasPrincipalKey(cm => new { cm.UserId, cm.ChatId })
                .OnDelete(DeleteBehavior.Restrict);

            builder
            .HasOne(x => x.Deleter)
            .WithMany()
            .HasForeignKey(x => x.DeletedBy)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(mr => new { mr.UserId, mr.ChatId });

            builder.ToTable("MessageReactions");
        }
    }


}

