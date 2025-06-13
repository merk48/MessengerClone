using MessengerClone.Domain.Common;
using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Domain.Entities
{
    public class UserLog : BaseEntity, ICreateAt
    {
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public enUserLogEvent Event { get; set; } // "Login", "Logout", "FailedLogin", etc.
        public string IpAddress { get; set; } = null!;
        public string UserAgent { get; set; } = null!;

        public DateTime CreatedAt { get; set; } // Timestamp of the event
    }
}
