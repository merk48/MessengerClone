using MessengerClone.Domain.Entities;
using MessengerClone.Service.Features.General.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessengerClone.Repository.EntityFrameworkCore.Configurations
{
    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(c => c.Type)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(c => c.Title)
                .HasMaxLength(ValidationHelper.MaxTitleLength)
                .IsRequired(false);

            builder.Property(x => x.ChatTheme)
                    .HasConversion<string>();

            builder.Property(c => c.CreatedAt)
               .IsRequired();

            // Configure LastMessage as an owned type
            builder.OwnsOne(c => c.LastMessage, lm =>
            {
                // Map each property of LastMessageSnapshot to its own column in Chats:
                lm.Property(x => x.Id)
                    .HasColumnName("LastMessageId");

                lm.Property(x => x.Content)
                    .HasColumnName("LastMessageContent")
                    .HasMaxLength(1000);

                lm.Property(x => x.SentAt)
                    .HasColumnName("LastMessageSentAt");

                lm.Property(x => x.SenderUserame)
                    .HasColumnName("LastMessageSenderUsername")
                    .HasMaxLength(100);

                lm.Property(x => x.Type)
                    .HasColumnName("LastMessageType")
                    .HasConversion<string>();
            });

            builder.Property(c => c.DateDeleted)
                .IsRequired(false);

            builder
            .HasOne(x => x.DeletedBy)
            .WithMany()
            .HasForeignKey(x => x.DeletedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);


            builder.UseTphMappingStrategy();

            builder.HasQueryFilter(c => !c.IsDeleted);

            builder.ToTable("Chats");
        }
    }


}

