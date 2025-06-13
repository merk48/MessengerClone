namespace MessengerClone.Domain.Abstractions
{
    public interface IUserContext
    {
        int UserId { get; }
        string? UserName { get; }
        IReadOnlyCollection<string> Roles { get; }
    }
}
