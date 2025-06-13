using MessengerClone.API.ConfigurationOptions;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace MessengerClone.Domain.Utils.Constants.SMS
{
    public class TwilioSmsService : ISmsSender
    {
        private readonly TwilioSettings _settings;

        public TwilioSmsService(IOptions<TwilioSettings> cfg)
        {
            _settings = cfg.Value;
            TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);
        }

        public Task SendSmsAsync(string toPhoneNumber, string message)
        {
            // returns a Task but Twilio’s Create call is synchronous; wrap in Task.Run
            return Task.Run(() =>
            {
                MessageResource.Create(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(_settings.FromNumber),
                    to: new Twilio.Types.PhoneNumber(toPhoneNumber)
                );
            });
        }
    }
}
