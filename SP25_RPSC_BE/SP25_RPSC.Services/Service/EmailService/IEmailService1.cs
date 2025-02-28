
namespace SP25_RPSC.Services.Service.EmailService
{
    public interface IEmailService
    {
        Task<bool> SendEmail(string Email, string Subject, string Html);
    }
}