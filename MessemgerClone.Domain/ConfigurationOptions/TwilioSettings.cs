namespace MessengerClone.API.ConfigurationOptions
{
    public class TwilioSettings
    {
        public string AccountSid { get; set; } = null!;
        public string AuthToken { get; set; } = null!;
        public string FromNumber { get; set; } = null!;
    }
}
