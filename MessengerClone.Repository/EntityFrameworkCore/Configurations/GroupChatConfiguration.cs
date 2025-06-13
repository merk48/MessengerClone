using MessengerClone.Domain.Entities;
using MessengerClone.Service.Features.General.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessengerClone.Repository.EntityFrameworkCore.Configurations
{
    public class GroupChatConfiguration : IEntityTypeConfiguration<GroupChat>
    {
        public void Configure(EntityTypeBuilder<GroupChat> builder)
        {
            builder.Property(gc => gc.GroupCoverImageUrl)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(gc => gc.Description)
                .HasMaxLength(ValidationHelper.MaxDescriptionLength)
                .IsRequired();

            builder.Property(c => c.CreatedBy)
             .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .IsRequired(false);

            builder.Property(c => c.UpdatedBy)
               .IsRequired(false);

            // Creator: one-to-many (required)
            builder.HasOne(gc => gc.Creator)
                   .WithMany(u => u.CreatedGroupConversations)
                   .HasForeignKey(gc => gc.CreatedBy)
                   .OnDelete(DeleteBehavior.NoAction);

            // Updater: one-to-many (optional)
            builder.HasOne(gc => gc.Updater)
                   .WithMany(u => u.UpdatedGroupConversations)
                   .HasForeignKey(gc => gc.UpdatedBy)
                   .OnDelete(DeleteBehavior.NoAction);


        }
    }


}

