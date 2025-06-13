using MessengerClone.Domain.Utils.Enums;

public class LastMessageSnapshot
{
    public int Id { get; set; }
    public string Content { get; set; } = default!;
    public DateTime SentAt { get; set; }
    public string SenderUserame { get; set; } = default!;
    public enMessageType Type { get; set; } = default!;
}
