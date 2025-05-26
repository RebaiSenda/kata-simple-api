using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace KataSimpleAPI.Services
{
    public class SmtpEmailSender : ISmtpEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<EmailSettings> emailSettings, ILogger<SmtpEmailSender> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string subject, string body, string toEmail)
        {
            try
            {
                _logger.LogInformation("=== DÉBUT ENVOI EMAIL ===");
                _logger.LogInformation("SMTP Server: {SmtpServer}:{Port}", _emailSettings.SmtpServer, _emailSettings.Port);
                _logger.LogInformation("Username: {Username}", _emailSettings.Username);
                _logger.LogInformation("From: {FromEmail}", _emailSettings.FromEmail);
                _logger.LogInformation("To: {ToEmail}", toEmail);
                _logger.LogInformation("Subject: {Subject}", subject);

                using var mail = new MailMessage();
                mail.From = new MailAddress(_emailSettings.FromEmail);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = false;

                using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port);

                // Configuration spécifique pour Mailtrap
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                smtpClient.EnableSsl = false; // Mailtrap sandbox n'utilise pas SSL sur le port 2525
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Timeout = 30000; // 30 secondes

                _logger.LogInformation("Configuration SMTP terminée, envoi en cours...");
                await smtpClient.SendMailAsync(mail);

                _logger.LogInformation("✅ Email envoyé avec succès à {ToEmail}", toEmail);
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "Erreur SMTP lors de l'envoi d'email à {ToEmail}: {Message}", toEmail, smtpEx.Message);
                _logger.LogError("Code d'erreur SMTP: {StatusCode}", smtpEx.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur générale lors de l'envoi d'email à {ToEmail}: {Message}", toEmail, ex.Message);
                throw;
            }
        }
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string ToEmail { get; set; } = string.Empty;
    }
}