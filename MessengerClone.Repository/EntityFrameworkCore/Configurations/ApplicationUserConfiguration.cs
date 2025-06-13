using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Service.Features.General.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessengerClone.Repository.EntityFrameworkCore.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName)
                .HasMaxLength(ValidationHelper.MaxNameLength)
                .IsRequired();

            builder.Property(u => u.LastName)
                .HasMaxLength(ValidationHelper.MaxNameLength)
                .IsRequired();

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);
        }
    }


}

