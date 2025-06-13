namespace MessengerClone.Domain.Utils.Constants.SMS
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string toPhoneNumber, string message);
    }

}
