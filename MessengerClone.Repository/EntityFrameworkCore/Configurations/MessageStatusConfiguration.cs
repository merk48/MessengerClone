using MessengerClone.Domain.Entities;
using MessengerClone.Domain.Utils.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessengerClone.Repository.EntityFrameworkCore.Configurations
{
    public class MessageStatusConfiguration : IEntityTypeConfiguration<MessageStatus>
    {
        public void Configure(EntityTypeBuilder<MessageStatus> builder)
        {
            builder.HasKey(ms => new { ms.MessageId, ms.UserId, ms.ChatId });

            builder.Property(ms => ms.Status)
                   .HasConversion<string>()
                   .HasDefaultValue(enMessageStatus.Sent);

            builder.Property(ms => ms.CreatedAt)
             .IsRequired();

            builder.Property(ms => ms.DeliveredAt)
            .IsRequired(false); 

            builder.Property(ms => ms.ReadAt)
             .IsRequired(false);

            builder
                .HasOne(mr => mr.Member)
                .WithMany(cm => cm.MessageStatuses)
                .HasForeignKey(x => new { x.UserId, x.ChatId })
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(ms => ms.Message)
                .WithMany(m => m.MessageStatuses)
                .HasForeignKey(ms => ms.MessageId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable("MessageStatuses");
        }
    }


}

