//namespace KataSimpleAPI.Services
//{
//    public class BookingConsumerHostedService : IHostedService
//    {
//        private readonly IBookingConsumerService _consumerService;
//        private readonly ILogger<BookingConsumerHostedService> _logger;

//        public BookingConsumerHostedService(
//            IBookingConsumerService consumerService,
//            ILogger<BookingConsumerHostedService> logger)
//        {
//            _consumerService = consumerService;
//            _logger = logger;
//        }

//        public Task StartAsync(CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Service de consommation des messages de réservation démarré");

//            _consumerService.StartConsuming();

//            return Task.CompletedTask;
//        }

//        public Task StopAsync(CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Arrêt du service de consommation des messages de réservation");

//            _consumerService.StopConsuming();

//            return Task.CompletedTask;
//        }
//    }
//}
