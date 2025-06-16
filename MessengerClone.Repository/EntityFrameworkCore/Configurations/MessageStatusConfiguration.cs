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
            builder.HasKey(mrr => new { mrr.MemberId, mrr.MessageId });

            builder.Property(mrr => mrr.Status)
                   .HasConversion<string>()
                   .HasDefaultValue(enMessageStatus.Sent);

            builder.Property(mrr => mrr.CreatedAt)
             .IsRequired();

            builder.Property(mrr => mrr.DeliveredAt)
            .IsRequired(false); 

            builder.Property(mrr => mrr.ReadAt)
             .IsRequired(false);

            builder
                .HasOne(mrr => mrr.Member)
                .WithMany(m => m.MessageInfo)
                .HasForeignKey(mrr => mrr.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(mrr => mrr.Message)
                .WithMany(m => m.MessageInfo)
                .HasForeignKey(mrr => mrr.MessageId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable("MessageStatuses");
        }
    }


}

