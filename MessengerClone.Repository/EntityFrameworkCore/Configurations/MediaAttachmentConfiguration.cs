using MessengerClone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessengerClone.Repository.EntityFrameworkCore.Configurations
{
    public class MediaAttachmentConfiguration : IEntityTypeConfiguration<MediaAttachment>
    {
        public void Configure(EntityTypeBuilder<MediaAttachment> builder)
        {
            builder.HasKey(ma => ma.MessageId);

            builder.Property(ma => ma.AttachmentUrl)
                   .HasMaxLength(2048)
                   .IsRequired();

            builder.Property(x => x.FileType)
                .HasConversion<string>();

            builder.Property(m => m.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(m => m.DateDeleted)
                .IsRequired(false);

            builder.HasOne(ma => ma.Message)
              .WithOne(m => m.Attachment)
              .HasForeignKey<MediaAttachment>(ma => ma.MessageId)
              .OnDelete(DeleteBehavior.Cascade);


            builder
             .HasOne(x => x.Deleter)
             .WithMany()
             .HasForeignKey(x => x.DeletedBy)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("MediaAttachments");
        }
    }


}

