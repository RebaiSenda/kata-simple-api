using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace KataSimpleAPI.Services
{
    public class BookingNotificationMessage
    {
        public int BookingId { get; set; }
        public int RoomId { get; set; }
        public int PersonId { get; set; }
        public DateTime BookingDate { get; set; }
        public int StartSlot { get; set; }
        public int EndSlot { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class RabbitMQConfig
    {
        public string HostName { get; set; } = "localhost";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string BookingExchange { get; set; } = "booking_exchange";
        public string BookingCreatedQueue { get; set; } = "booking_created_queue";
        public string BookingCreatedRoutingKey { get; set; } = "booking.created";
    }

    public class RabbitMQConsumerService : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQConfig _config;
        private readonly ILogger<RabbitMQConsumerService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQConsumerService(
            IOptions<RabbitMQConfig> config,
            ILogger<RabbitMQConsumerService> logger,
            IServiceProvider serviceProvider)
        {
            _config = config.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _config.HostName,
                    UserName = _config.Username,
                    Password = _config.Password
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Déclarer l'exchange
                _channel.ExchangeDeclare(
                    exchange: _config.BookingExchange,
                    type: ExchangeType.Direct,
                    durable: true);

                // Déclarer la queue
                _channel.QueueDeclare(
                    queue: _config.BookingCreatedQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                // Lier la queue à l'exchange
                _channel.QueueBind(
                    queue: _config.BookingCreatedQueue,
                    exchange: _config.BookingExchange,
                    routingKey: _config.BookingCreatedRoutingKey);

                _logger.LogInformation("Connected to RabbitMQ consumer and configured exchange/queue");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to RabbitMQ");
                throw;
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (_, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var bookingNotification = JsonSerializer.Deserialize<BookingNotificationMessage>(message);

                    if (bookingNotification != null)
                    {
                        _logger.LogInformation("Message reçu: BookingId {BookingId}, Status {Status}",
                            bookingNotification.BookingId, bookingNotification.Status);

                        // Traiter le message selon le statut
                        switch (bookingNotification.Status)
                        {
                            case "Created":
                                ProcessBookingCreated(bookingNotification);
                                break;
                            case "Deleted":
                                ProcessBookingDeleted(bookingNotification.BookingId);
                                break;
                            default:
                                _logger.LogWarning("Status de réservation non reconnu: {Status}", bookingNotification.Status);
                                break;
                        }
                    }

                    // Acknowledge le message
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors du traitement du message RabbitMQ");
                    // En cas d'erreur, on peut soit rejeter le message, soit le remettre dans la queue
                    _channel.BasicNack(ea.DeliveryTag, false, true); // Remet le message dans la queue
                }
            };

            _channel.BasicConsume(
                queue: _config.BookingCreatedQueue,
                autoAck: false, // Important: on veut confirmer manuellement
                consumer: consumer);

            return Task.CompletedTask;
        }

        private void ProcessBookingCreated(BookingNotificationMessage booking)
        {
            // Traitement lors de la création d'une réservation
            _logger.LogInformation("Traitement de la création de réservation: ID {BookingId}, Salle {RoomId}, Personne {PersonId}",
                booking.BookingId, booking.RoomId, booking.PersonId);

            // Ici, vous pourriez implémenter:
            // - Stockage en base de données
            // - Envoi d'email de confirmation
            // - Mise à jour des statistiques d'occupation, etc.
        }

        private void ProcessBookingDeleted(int bookingId)
        {
            // Traitement lors de la suppression d'une réservation
            _logger.LogInformation("Traitement de la suppression de réservation: ID {BookingId}", bookingId);

            // Ici, vous pourriez implémenter:
            // - Suppression de la réservation en base de données
            // - Envoi d'email d'annulation
            // - Mise à jour des statistiques d'occupation, etc.
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}

