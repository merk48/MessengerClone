using MessengerClone.Domain.Entities;
using MessengerClone.Domain.Entities.Identity;

namespace MessengerClone.Domain.Common.Interfaces
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DateDeleted { get; set; }
        int? DeletedBy { get; set; }
        ApplicationUser? Deleter { get; set; }

        void Delete()
        {
            IsDeleted = true;
            DateDeleted = DateTime.UtcNow;
        }

        void UndoDelete()
        {
            IsDeleted = false;
            DateDeleted = null;
            DeletedBy = null;
        }
    }
    
}
