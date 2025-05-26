using KataSimpleAPI.Models;

namespace KataSimpleAPI.Services
{
    public static class StaticEmailService
    {
        public static async Task SendUpdateEmailAsync(BookingMessage booking)
        {
            await Task.Delay(200); // Simulation
            Console.WriteLine($"[EMAIL SIMULÉ] Réservation {booking.BookingId} mise à jour.");
            Console.WriteLine($"  - Nouvelle salle: {booking.RoomId}");
            Console.WriteLine($"  - Nouvelle date: {booking.BookingDate.ToShortDateString()}");
            Console.WriteLine($"  - Nouveaux créneaux: {booking.StartSlot}-{booking.EndSlot}");
        }

        public static async Task SendCancellationEmailAsync(BookingMessage booking)
        {
            await Task.Delay(200); // Simulation d'envoi d'email
            Console.WriteLine($"[EMAIL ANNULATION] Réservation {booking.BookingId} annulée");
        }

        public static async Task SendUpdateEmailAsync(BookingMessage booking, ISmtpEmailSender smtpEmailSender)
        {
            try
            {
                var subject = "Mise à jour de Réservation";
                var body = $"Votre réservation {booking.BookingId} a été mise à jour.\n\n" +
                           $"Détails:\n" +
                           $"- Salle: {booking.RoomId}\n" +
                           $"- Date: {booking.BookingDate.ToShortDateString()}\n" +
                           $"- Créneaux: {booking.StartSlot}-{booking.EndSlot}\n" +
                           $"- Personne: {booking.PersonId}";

                string toEmail = "noreply@outlook.com";

                await smtpEmailSender.SendEmailAsync(subject, body, toEmail);
                Console.WriteLine($"[EMAIL ENVOYÉ] Mise à jour de réservation {booking.BookingId} envoyée à {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERREUR EMAIL] Impossible d'envoyer l'email pour la réservation {booking.BookingId}: {ex.Message}");
                throw; // Re-throw pour que RabbitMQ puisse gérer le retry
            }
        }

        // Nouvelles méthodes pour les autres types d'emails
        public static async Task SendConfirmationEmailAsync(BookingMessage booking, ISmtpEmailSender smtpEmailSender)
        {
            try
            {
                var subject = "Confirmation de Réservation";
                var body = $"Votre réservation {booking.BookingId} est confirmée.\n\n" +
                           $"Détails:\n" +
                           $"- Salle: {booking.RoomId}\n" +
                           $"- Date: {booking.BookingDate.ToShortDateString()}\n" +
                           $"- Créneaux: {booking.StartSlot}-{booking.EndSlot}\n" +
                           $"- Personne: {booking.PersonId}";

                string toEmail = "noreply@outlook.com";

                await smtpEmailSender.SendEmailAsync(subject, body, toEmail);
                Console.WriteLine($"[EMAIL ENVOYÉ] Confirmation de réservation {booking.BookingId} envoyée à {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERREUR EMAIL] Impossible d'envoyer l'email de confirmation pour la réservation {booking.BookingId}: {ex.Message}");
                throw;
            }
        }

        public static async Task SendCancellationEmailAsync(BookingMessage booking, ISmtpEmailSender smtpEmailSender)
        {
            try
            {
                var subject = "Annulation de Réservation";
                var body = $"Votre réservation {booking.BookingId} a été annulée.\n\n" +
                           $"Détails de la réservation annulée:\n" +
                           $"- Salle: {booking.RoomId}\n" +
                           $"- Date: {booking.BookingDate.ToShortDateString()}\n" +
                           $"- Créneaux: {booking.StartSlot}-{booking.EndSlot}\n" +
                           $"- Personne: {booking.PersonId}";

                string toEmail = "noreply@outlook.com";

                await smtpEmailSender.SendEmailAsync(subject, body, toEmail);
                Console.WriteLine($"[EMAIL ENVOYÉ] Annulation de réservation {booking.BookingId} envoyée à {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERREUR EMAIL] Impossible d'envoyer l'email d'annulation pour la réservation {booking.BookingId}: {ex.Message}");
                throw;
            }
        }
    }
}