using KataSimpleAPI.Models;

namespace KataSimpleAPI.Services
{
    // Interface principale du processeur
    public interface IFakeBookingProcessor
    {
        Task ProcessBooking(BookingMessage bookingMessage);
    }

    // Interfaces pour les services
    public interface IBookingRepository
    {
        Task<BookingMessage?> GetByIdAsync(int bookingId);
        Task CreateAsync(BookingMessage booking);
        Task UpdateAsync(BookingMessage booking);
        Task DeleteAsync(int bookingId);
    }

    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(BookingMessage booking);
        Task SendCancellationEmailAsync(BookingMessage booking);
        Task SendUpdateEmailAsync(BookingMessage booking);
    }

    public interface IStatisticsService
    {
        Task UpdateBookingStatisticsAsync(BookingMessage booking, string action);
    }

    public interface INotificationService
    {
        Task SendPushNotificationAsync(BookingMessage booking, string action);
    }

    // Implémentations des services
    public class BookingRepository : IBookingRepository
    {
        private readonly ILogger<BookingRepository> _logger;
        // Dans un vrai projet, vous auriez un DbContext ici
        private static readonly Dictionary<int, BookingMessage> _bookings = new();

        public BookingRepository(ILogger<BookingRepository> logger)
        {
            _logger = logger;
        }

        public async Task<BookingMessage?> GetByIdAsync(int bookingId)
        {
            await Task.Delay(50); // Simulation latence DB
            _bookings.TryGetValue(bookingId, out var booking);
            _logger.LogDebug("Récupération booking {BookingId} depuis la DB", bookingId);
            return booking;
        }

        public async Task CreateAsync(BookingMessage booking)
        {
            await Task.Delay(100); // Simulation latence DB
            _bookings[booking.BookingId] = booking;
            _logger.LogInformation("Booking {BookingId} créé en base de données", booking.BookingId);
        }

        public async Task UpdateAsync(BookingMessage booking)
        {
            await Task.Delay(100); // Simulation latence DB
            _bookings[booking.BookingId] = booking;
            _logger.LogInformation("Booking {BookingId} mis à jour en base de données", booking.BookingId);
        }

        public async Task DeleteAsync(int bookingId)
        {
            await Task.Delay(100); // Simulation latence DB
            _bookings.Remove(bookingId);
            _logger.LogInformation("Booking {BookingId} supprimé de la base de données", bookingId);
        }
    }

    public class EmailService : IEmailService
    {
        private readonly ISmtpEmailSender _smtpEmailSender;

        public EmailService(ISmtpEmailSender smtpEmailSender)
        {
            _smtpEmailSender = smtpEmailSender;
        }

        public async Task SendConfirmationEmailAsync(BookingMessage booking)
        {
            var subject = "Confirmation de Réservation";
            var body = $"Votre réservation {booking.BookingId} est confirmée\n" +
                       $"Salle: {booking.RoomId}\nDate: {booking.BookingDate.ToShortDateString()}\n" +
                       $"Créneaux: {booking.StartSlot}-{booking.EndSlot}";

            string toEmail = "noreply@outlook.com"; // Remplace ça !

            await _smtpEmailSender.SendEmailAsync(subject, body, toEmail);
        }

        public async Task SendCancellationEmailAsync(BookingMessage booking)
        {
            string subject = "Annulation de Réservation";
            string body = $"Votre réservation {booking.BookingId} a été annulée.";
            string toEmail = "ton.email.pro@example.com";

            await _smtpEmailSender.SendEmailAsync(subject, body, toEmail);
        }

        public async Task SendUpdateEmailAsync(BookingMessage booking)
        {
            string subject = "Mise à jour de Réservation";
            string body = $"Votre réservation {booking.BookingId} a été modifiée\n" +
                          $"Salle: {booking.RoomId}\nDate: {booking.BookingDate.ToShortDateString()}\n" +
                          $"Créneaux: {booking.StartSlot}-{booking.EndSlot}";

            string toEmail = "ton.email.pro@example.com";

            await _smtpEmailSender.SendEmailAsync(subject, body, toEmail);
        }
    }

    // Classe statique pour les emails
    public static class StaticEmailHelper
    {
        public static async Task SendConfirmationEmailAsync(BookingMessage booking, ILogger? logger = null)
        {
            await Task.Delay(200); // Simulation envoi email

            var message = $"Email de confirmation envoyé pour la réservation {booking.BookingId}, " +
                         $"Salle {booking.RoomId}, Date {booking.BookingDate.ToShortDateString()}, " +
                         $"Créneaux {booking.StartSlot}-{booking.EndSlot}";

            logger?.LogInformation(message);
            Console.WriteLine($"[EMAIL] {message}");
        }

        public static async Task SendCancellationEmailAsync(BookingMessage booking, ILogger? logger = null)
        {
            await Task.Delay(200); // Simulation envoi email

            var message = $"Email d'annulation envoyé pour la réservation {booking.BookingId}";

            logger?.LogInformation(message);
            Console.WriteLine($"[EMAIL] {message}");
        }

        public static async Task SendUpdateEmailAsync(BookingMessage booking, ILogger? logger = null)
        {
            await Task.Delay(200); // Simulation envoi email

            var message = $"Email de modification envoyé pour la réservation {booking.BookingId}, " +
                         $"Nouvelles informations: Salle {booking.RoomId}, Date {booking.BookingDate.ToShortDateString()}, " +
                         $"Créneaux {booking.StartSlot}-{booking.EndSlot}";

            logger?.LogInformation(message);
            Console.WriteLine($"[EMAIL] {message}");
        }
    }

    public class StatisticsService : IStatisticsService
    {
        private readonly ILogger<StatisticsService> _logger;
        // Simulation de stockage des statistiques
        private static readonly Dictionary<string, int> _stats = new();

        public StatisticsService(ILogger<StatisticsService> logger)
        {
            _logger = logger;
        }

        public async Task UpdateBookingStatisticsAsync(BookingMessage booking, string action)
        {
            await Task.Delay(50); // Simulation traitement stats

            var key = $"{action.ToLower()}_bookings";
            _stats.TryGetValue(key, out var currentCount);
            _stats[key] = currentCount + 1;

            // Statistiques par salle
            var roomKey = $"room_{booking.RoomId}_{action.ToLower()}";
            _stats.TryGetValue(roomKey, out var roomCount);
            _stats[roomKey] = roomCount + 1;

            _logger.LogInformation("Statistique mise à jour pour la réservation {BookingId} avec l'action {Action}", booking.BookingId, action);
        }
    }

    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public async Task SendPushNotificationAsync(BookingMessage booking, string action)
        {
            await Task.Delay(150); // Simulation envoi notification

            var message = action.ToLower() switch
            {
                "created" => $"Votre réservation de la salle {booking.RoomId} est confirmée",
                "updated" => $"Votre réservation de la salle {booking.RoomId} a été modifiée",
                "deleted" => $"Votre réservation de la salle {booking.RoomId} a été annulée",
                _ => $"Mise à jour de votre réservation {booking.BookingId}"
            };

            _logger.LogInformation("Notification push envoyée à la personne {PersonId}: {Message}",
                booking.PersonId, message);
        }
    }

    // Version améliorée du FakeBookingProcessor
    public class FakeBookingProcessor : IFakeBookingProcessor
    {
        private readonly ILogger<FakeBookingProcessor> _logger;
        private readonly IBookingRepository _bookingRepository;
        private readonly IStatisticsService _statisticsService;
        private readonly INotificationService _notificationService;

        public FakeBookingProcessor(
            ILogger<FakeBookingProcessor> logger,
            IBookingRepository bookingRepository,
            IStatisticsService statisticsService,
            INotificationService notificationService)
        {
            _logger = logger;
            _bookingRepository = bookingRepository;
            _statisticsService = statisticsService;
            _notificationService = notificationService;
        }

        public async Task ProcessBooking(BookingMessage bookingMessage)
        {
            _logger.LogInformation("Début du traitement de la réservation: {BookingId}, Status: {Status}",
                bookingMessage.BookingId, bookingMessage.Status);

            try
            {
                // Simuler un délai de traitement asynchrone
                await Task.Delay(500);

                switch (bookingMessage.Status.ToLower())
                {
                    case "created":
                        await ProcessBookingCreated(bookingMessage);
                        break;
                    case "deleted":
                        await ProcessBookingDeleted(bookingMessage);
                        break;
                    case "updated":
                        await ProcessBookingUpdated(bookingMessage);
                        break;
                    default:
                        _logger.LogWarning("Status de réservation non reconnu: {Status}", bookingMessage.Status);
                        break;
                }

                _logger.LogInformation("Traitement terminé avec succès pour la réservation {BookingId}",
                    bookingMessage.BookingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du traitement de la réservation {BookingId}",
                    bookingMessage.BookingId);
                throw; // Re-throw pour permettre le retry par RabbitMQ
            }
        }

        private async Task ProcessBookingCreated(BookingMessage booking)
        {
            _logger.LogInformation("Traitement création de réservation: ID {BookingId}, Salle {RoomId}, " +
                "Personne {PersonId}, Date: {Date}, Créneau: {StartSlot}-{EndSlot}",
                booking.BookingId, booking.RoomId, booking.PersonId,
                booking.BookingDate.ToShortDateString(), booking.StartSlot, booking.EndSlot);

            // Exécution parallèle des opérations non-dépendantes
            var tasks = new List<Task>
            {
                _bookingRepository.CreateAsync(booking),
                StaticEmailHelper.SendConfirmationEmailAsync(booking), // Appel statique
                _statisticsService.UpdateBookingStatisticsAsync(booking, "created"),
                _notificationService.SendPushNotificationAsync(booking, "created")
            };

            await Task.WhenAll(tasks);
            _logger.LogInformation("Création de réservation {BookingId} traitée avec succès", booking.BookingId);
        }

        private async Task ProcessBookingDeleted(BookingMessage booking)
        {
            _logger.LogInformation("Traitement suppression de réservation: ID {BookingId}", booking.BookingId);

            // Récupérer les détails depuis la DB pour les emails/notifications
            var existingBooking = await _bookingRepository.GetByIdAsync(booking.BookingId);
            if (existingBooking == null)
            {
                _logger.LogWarning("Tentative de suppression d'une réservation inexistante: {BookingId}",
                    booking.BookingId);
                return;
            }

            // Exécution parallèle des opérations
            var tasks = new List<Task>
            {
                _bookingRepository.DeleteAsync(booking.BookingId),
                StaticEmailService.SendCancellationEmailAsync(existingBooking), // Appel statique
                _statisticsService.UpdateBookingStatisticsAsync(existingBooking, "deleted"),
                _notificationService.SendPushNotificationAsync(existingBooking, "deleted")
            };

            await Task.WhenAll(tasks);
            _logger.LogInformation("Suppression de réservation {BookingId} traitée avec succès", booking.BookingId);
        }

        private async Task ProcessBookingUpdated(BookingMessage booking)
        {
            _logger.LogInformation("Traitement modification de réservation: ID {BookingId}, Salle {RoomId}, " +
                "Personne {PersonId}, Date: {Date}, Créneau: {StartSlot}-{EndSlot}",
                booking.BookingId, booking.RoomId, booking.PersonId,
                booking.BookingDate.ToShortDateString(), booking.StartSlot, booking.EndSlot);

            // Exécution parallèle des opérations
            var tasks = new List<Task>
            {
                _bookingRepository.UpdateAsync(booking),
                StaticEmailService.SendUpdateEmailAsync(booking), // Appel statique
                _statisticsService.UpdateBookingStatisticsAsync(booking, "updated"),
                _notificationService.SendPushNotificationAsync(booking, "updated")
            };

            await Task.WhenAll(tasks);
            _logger.LogInformation("Modification de réservation {BookingId} traitée avec succès", booking.BookingId);
        }
    }
}