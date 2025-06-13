namespace MessengerClone.Domain.Common.Interfaces
{
    public interface IEntity<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
    }
}
