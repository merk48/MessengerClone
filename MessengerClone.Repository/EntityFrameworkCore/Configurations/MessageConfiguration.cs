using MessengerClone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessengerClone.Repository.EntityFrameworkCore.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Content)
               .IsRequired();

            builder.Property(m => m.Type)
               .HasConversion<string>() 
               .IsRequired();

            builder.Property(m => m.CreatedAt)
               .IsRequired();

            builder.Property(m => m.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(m => m.DateDeleted)
                .IsRequired(false);

            builder.Property(m => m.ChatId)
               .IsRequired();

            builder.Property(m => m.SenderId)
               .IsRequired();

            builder
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
              .HasOne(m => m.PinnedByUser)
              .WithMany()
              .HasForeignKey(m => m.PinnedById)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => m.CreatedAt);

            builder.ToTable("Messages");
        }
    }


}

