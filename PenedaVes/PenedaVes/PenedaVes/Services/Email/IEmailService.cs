using System.Threading.Tasks;

namespace PenedaVes.Services.Email
{
    public interface IEmailService
    {
        Task SendEmail(string email, string subject, string message);
    }
}