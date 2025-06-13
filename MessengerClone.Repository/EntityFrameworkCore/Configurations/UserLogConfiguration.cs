using MessengerClone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessengerClone.Repository.EntityFrameworkCore.Configurations
{
    public class UserLogConfiguration : IEntityTypeConfiguration<UserLog>
    {
        public void Configure(EntityTypeBuilder<UserLog> builder)
        {
            builder.Property(ul => ul.UserId)
                .IsRequired();

            builder.Property(ul => ul.IpAddress)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(ul => ul.UserAgent)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(ul => ul.Event)
            .HasConversion<string>() 
            .IsRequired();

            builder.Property(ul => ul.CreatedAt)
             .IsRequired();

            builder
              .HasOne(ul => ul.User)
              .WithMany()
              .HasForeignKey(ul => ul.UserId)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }


}

