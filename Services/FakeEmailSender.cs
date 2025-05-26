using Microsoft.Extensions.Options;

namespace KataSimpleAPI.Services
{
    // Email sender de simulation pour les tests
    public class FakeEmailSender : ISmtpEmailSender
    {
        private readonly ILogger<FakeEmailSender> _logger;

        public FakeEmailSender(ILogger<FakeEmailSender> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string subject, string body, string toEmail)
        {
            await Task.Delay(500); // Simulation latence réseau

            _logger.LogInformation("📧 [EMAIL SIMULÉ] ================================");
            _logger.LogInformation("📧 De: noreply@katasimpleapi.com");
            _logger.LogInformation("📧 À: {ToEmail}", toEmail);
            _logger.LogInformation("📧 Sujet: {Subject}", subject);
            _logger.LogInformation("📧 Corps du message:");
            _logger.LogInformation("📧 {Body}", body);
            _logger.LogInformation("📧 ===============================================");

            // Écrire aussi dans la console
            Console.WriteLine($"\n📧 EMAIL ENVOYÉ AVEC SUCCÈS");
            Console.WriteLine($"À: {toEmail}");
            Console.WriteLine($"Sujet: {subject}");
            Console.WriteLine($"Message: {body}");
            Console.WriteLine("=" + new string('=', 50));
        }
    }
}