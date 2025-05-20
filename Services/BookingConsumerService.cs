//using System.Text;
//using System.Text.Json;
//using Microsoft.Extensions.Options;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//namespace KataSimpleAPI.Services
//{
//    public class RabbitMQConfig
//    {
//        public string HostName { get; set; } = "localhost";
//        public string Username { get; set; } = "guest";
//        public string Password { get; set; } = "guest";
//        public string BookingExchange { get; set; } = "booking_exchange";
//        public string BookingCreatedQueue { get; set; } = "booking_created_queue";
//        public string BookingCreatedRoutingKey { get; set; } = "booking.created";
//    }

//    public class BookingMessage
//    {
//        public int BookingId { get; set; }
//        public int RoomId { get; set; }
//        public int PersonId { get; set; }
//        public DateTime BookingDate { get; set; }
//        public int StartSlot { get; set; }
//        public int EndSlot { get; set; }
//        public string Status { get; set; } = string.Empty;
//    }

//    public interface IBookingConsumerService
//    {
//        void StartConsuming();
//        void StopConsuming();
//    }

//    public class BookingConsumerService : IBookingConsumerService, IDisposable
//    {
//        private readonly IConnection _connection;
//        private readonly IModel _channel;
//        private readonly RabbitMQConfig _config;
//        private readonly ILogger<BookingConsumerService> _logger;
//        private readonly IServiceProvider _serviceProvider;

//        public BookingConsumerService(
//            IOptions<RabbitMQConfig> config,
//            ILogger<BookingConsumerService> logger,
//            IServiceProvider serviceProvider)
//        {
//            _config = config.Value;
//            _logger = logger;
//            _serviceProvider = serviceProvider;

//            try
//            {
//                var factory = new ConnectionFactory
//                {
//                    HostName = _config.HostName,
//                    UserName = _config.Username,
//                    Password = _config.Password
//                };

//                _connection = factory.CreateConnection();
//                _channel = _connection.CreateModel();

//                // Déclarer l'exchange
//                _channel.ExchangeDeclare(
//                    exchange: _config.BookingExchange,
//                    type: ExchangeType.Direct,
//                    durable: true);

//                // Déclarer la queue
//                _channel.QueueDeclare(
//                    queue: _config.BookingCreatedQueue,
//                    durable: true,
//                    exclusive: false,
//                    autoDelete: false);

//                // Lier la queue à l'exchange
//                _channel.QueueBind(
//                    queue: _config.BookingCreatedQueue,
//                    exchange: _config.BookingExchange,
//                    routingKey: _config.BookingCreatedRoutingKey);

//                _logger.LogInformation("Connected to RabbitMQ and configured consumer");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Failed to connect to RabbitMQ");
//                throw;
//            }
//        }

//        public void StartConsuming()
//        {
//            _logger.LogInformation("Starting to consume messages from {QueueName}", _config.BookingCreatedQueue);

//            // Configuration de Quality of Service (QoS)
//            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

//            var consumer = new EventingBasicConsumer(_channel);
//            consumer.Received += (sender, args) =>
//            {
//                try
//                {
//                    var body = args.Body.ToArray();
//                    var message = Encoding.UTF8.GetString(body);

//                    _logger.LogInformation("Message reçu: {Message}", message);

//                    // Désérialiser le message
//                    var bookingMessage = JsonSerializer.Deserialize<BookingMessage>(message);

//                    if (bookingMessage != null)
//                    {
//                        // Service "fake" pour traiter le message de réservation
//                        using (var scope = _serviceProvider.CreateScope())
//                        {
//                            var fakeService = scope.ServiceProvider.GetRequiredService<IFakeBookingProcessor>();
//                            fakeService.ProcessBooking(bookingMessage);
//                        }
//                    }

//                    // Confirmer la réception du message
//                    _channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Erreur lors du traitement du message");
//                    // En cas d'erreur, rejeter le message et le remettre dans la queue
//                    _channel.BasicNack(deliveryTag: args.DeliveryTag, multiple: false, requeue: true);
//                }
//            };

//            // Commencer à consommer les messages de la queue
//            _channel.BasicConsume(
//                queue: _config.BookingCreatedQueue,
//                autoAck: false, // Nous allons confirmer manuellement
//                consumer: consumer);
//        }

//        public void StopConsuming()
//        {
//            _channel?.Close();
//            _connection?.Close();
//        }

//        public void Dispose()
//        {
//            _channel?.Dispose();
//            _connection?.Dispose();
//        }
//    }
    

//}
