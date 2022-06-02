namespace TouchReceiver
{
    /// <summary>
    /// The ReceiverWorker class's purpose is to allow
    /// the ReceiverService to run as a Windows service
    /// </summary>
    public class ReceiverWorker : BackgroundService
    {
        private readonly ILogger<ReceiverWorker> _logger;
        private readonly ReceiverService _service;

        /// <summary>
        /// Constructor for the Worker object
        /// </summary>
        /// <param name="logger">Logger that the worker will use to log events</param>
        /// <param name="service">Receiver service that the worker will use</param>
        public ReceiverWorker(ILogger<ReceiverWorker> logger, ReceiverService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// Starts the receiver service that will handle receiving TUIO events and 
        /// converting them to touch events
        /// </summary>
        /// <param name="cancellationToken">Token that is used to determine whether the worker should stop</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Starting Receiver");
                _service.Start();
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.Log(LogLevel.Information, "Stopping Receiver");
                _service.Stop();
            }         
        }
    }
}