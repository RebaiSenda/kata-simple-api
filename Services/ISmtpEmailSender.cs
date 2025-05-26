namespace KataSimpleAPI.Services
{
    public interface ISmtpEmailSender
    {
        Task SendEmailAsync(string subject, string body, string toEmail);
    }
}
