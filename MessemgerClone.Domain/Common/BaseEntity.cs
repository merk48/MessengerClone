using MessengerClone.Domain.Common.Interfaces;

namespace MessengerClone.Domain.Common
{
    public abstract class BaseEntity : IEntity<int>
    {
        public int Id { get; set; } 
    }
}
