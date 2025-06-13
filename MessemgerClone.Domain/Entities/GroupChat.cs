using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.Entities.Identity;

namespace MessengerClone.Domain.Entities
{
    public class GroupChat : Chat , ICreatedBy ,IUpdateAt , IUpdatedBy
    {
        public string GroupCoverImageUrl { get; set; } = null!;
        public string? Description { get; set; }
      
        public int CreatedBy { get; set; }
        public ApplicationUser Creator { get; set; } = null!;

        // Last updater
        public int? UpdatedBy { get; set; }
        public ApplicationUser? Updater { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
