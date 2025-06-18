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
                 lm.Property(x => x.Id)
                   .HasColumnName("LastMessageId");

                 lm.Property(x => x.Content)
                   .HasColumnName("LastMessageContent");

                 lm.Property(x => x.SentAt)
                   .HasColumnName("LastMessageSentAt");

                 lm.Property(x => x.SenderUsername)   // EXACT match to CLR
                   .HasColumnName("LastMessageSenderUsername");

                 lm.Property(x => x.Type)
                   .HasColumnName("LastMessageType")
                   .HasConversion<string>();
             });


            builder.Property(c => c.DateDeleted)
                .IsRequired(false);

            builder
            .HasOne(x => x.Deleter)
            .WithMany()
            .HasForeignKey(x => x.DeletedBy)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);


            builder.UseTphMappingStrategy();

            builder.HasQueryFilter(c => !c.IsDeleted);

            builder.ToTable("Chats");
        }
    }


}

