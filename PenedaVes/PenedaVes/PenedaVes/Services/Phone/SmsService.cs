using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PenedaVes.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace PenedaVes.Services.Phone
{   
    public class SmsService : ISmsService

    {
        private Twilio.Types.PhoneNumber number;
        
        public SmsService(IOptions<TwilioSettings> options)
        {
            TwilioSettings settings = options.Value;
            number = new Twilio.Types.PhoneNumber(settings.From);

            string accountSid = settings.AccountSid;
            string authToken = settings.AuthToken;

            TwilioClient.Init(accountSid, authToken);
        }

        public Task<MessageResource> SendSms(string to, string body)
        {
            return MessageResource.CreateAsync(
                body: body,
                from: number,
                to: new Twilio.Types.PhoneNumber(to)
            );
        }
    }
}