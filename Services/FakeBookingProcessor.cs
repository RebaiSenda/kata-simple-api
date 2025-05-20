namespace KataSimpleAPI.Services
{
    public interface IFakeBookingProcessor
    {
        void ProcessBooking(BookingMessage bookingMessage);
    }

    public class FakeBookingProcessor : IFakeBookingProcessor
    {
        private readonly ILogger<FakeBookingProcessor> _logger;

        public FakeBookingProcessor(ILogger<FakeBookingProcessor> logger)
        {
            _logger = logger;
        }

        public void ProcessBooking(BookingMessage bookingMessage)
        {
            _logger.LogInformation("Traitement d'une nouvelle réservation: {BookingId}, Status: {Status}",
                bookingMessage.BookingId, bookingMessage.Status);

            // Simuler un délai de traitement
            Thread.Sleep(500);

            if (bookingMessage.Status == "Created")
            {
                _logger.LogInformation("Réservation {BookingId} créée et traitée avec succès pour la salle {RoomId} " +
                    "et la personne {PersonId}, Date: {Date}, Créneau: {StartSlot}-{EndSlot}",
                    bookingMessage.BookingId, bookingMessage.RoomId, bookingMessage.PersonId,
                    bookingMessage.BookingDate.ToShortDateString(), bookingMessage.StartSlot, bookingMessage.EndSlot);
            }
            else if (bookingMessage.Status == "Deleted")
            {
                _logger.LogInformation("Réservation {BookingId} supprimée", bookingMessage.BookingId);
            }
        }
    }
}
