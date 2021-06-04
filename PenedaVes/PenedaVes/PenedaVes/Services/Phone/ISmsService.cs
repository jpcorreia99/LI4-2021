using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace PenedaVes.Services.Phone
{
    public interface ISmsService
    {
        public Task<MessageResource> SendSms(string to, string body);
    }
}