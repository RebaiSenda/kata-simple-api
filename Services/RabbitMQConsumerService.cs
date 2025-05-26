using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using KataSimpleAPI.Models;

namespace KataSimpleAPI.Services
{
    public class RabbitMQConfig
    {
        public string HostName { get; set; } = "localhost";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string BookingExchange { get; set; } = "booking_exchange";
        public string BookingCreatedQueue { get; set; } = "booking_created_queue";
        public string BookingDeletedQueue { get; set; } = "booking_deleted_queue";
        public string BookingUpdatedQueue { get; set; } = "booking_updated_queue";
        public string BookingCreatedRoutingKey { get; set; } = "booking.created";
        public string BookingDeletedRoutingKey { get; set; } = "booking.deleted";
        public string BookingUpdatedRoutingKey { get; set; } = "booking.updated";
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

                // Configurer toutes les queues
                SetupQueues();

                _logger.LogInformation("Connected to RabbitMQ consumer and configured exchange/queues");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to RabbitMQ");
                throw;
            }
        }

        private void SetupQueues()
        {
            // Configuration des queues avec leurs routing keys
            var queueConfigs = new[]
            {
                (_config.BookingCreatedQueue, _config.BookingCreatedRoutingKey),
                (_config.BookingDeletedQueue, _config.BookingDeletedRoutingKey),
                (_config.BookingUpdatedQueue, _config.BookingUpdatedRoutingKey)
            };

            foreach (var (queueName, routingKey) in queueConfigs)
            {
                _channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                _channel.QueueBind(
                    queue: queueName,
                    exchange: _config.BookingExchange,
                    routingKey: routingKey);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            // Configurer le consumer pour toutes les queues
            SetupConsumers();

            return Task.CompletedTask;
        }

        private void SetupConsumers()
        {
            var queues = new[]
            {
                _config.BookingCreatedQueue,
                _config.BookingDeletedQueue,
                _config.BookingUpdatedQueue
            };

            foreach (var queue in queues)
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (_, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var bookingMessage = JsonSerializer.Deserialize<BookingMessage>(message);

                        if (bookingMessage != null)
                        {
                            _logger.LogInformation("Message reçu de {Queue}: BookingId {BookingId}, Status {Status}",
                                queue, bookingMessage.BookingId, bookingMessage.Status);

                            // Traitement asynchrone avec scope de DI
                            using var scope = _serviceProvider.CreateScope();
                            var processor = scope.ServiceProvider.GetRequiredService<IFakeBookingProcessor>(); 
                            var smtpEmailSender = scope.ServiceProvider.GetRequiredService<ISmtpEmailSender>();
                            await StaticEmailService.SendUpdateEmailAsync(bookingMessage, smtpEmailSender);
                            await Task.Run(() => processor.ProcessBooking(bookingMessage));
                        }

                        // Acknowledge le message
                        _channel.BasicAck(ea.DeliveryTag, false);

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erreur lors du traitement du message RabbitMQ de {Queue}", queue);
                        // Rejeter le message et le remettre dans la queue pour retry
                        _channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                };

                _channel.BasicConsume(
                    queue: queue,
                    autoAck: false,
                    consumer: consumer);
            }
        }

        public override void Dispose()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during RabbitMQ cleanup");
            }
            finally
            {
                _channel?.Dispose();
                _connection?.Dispose();
                base.Dispose();
            }
        }
    }
}