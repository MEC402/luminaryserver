namespace TouchProxy
{
    /// <summary>
    /// The ProxyWorker class's purpose is to allow
    /// the ProxyService to run as a Windows service
    /// </summary>
    public class ProxyWorker : BackgroundService
    {
        private readonly ILogger<ProxyWorker> _logger;
        private readonly ProxyService _service;

        /// <summary>
        /// Constructor for the ProxyWorker object
        /// </summary>
        /// <param name="logger">Logger that the proxy will use to log events</param>
        /// <param name="service">Proxy service that the worker will use</param>
        public ProxyWorker(ILogger<ProxyWorker> logger, ProxyService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// Starts the proxy service that will handle receiving and proxying
        /// TUIO events.
        /// </summary>
        /// <param name="cancellationToken">Token that is used to determine whether the worker should stop</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Starting Proxy");
                _service.Start();
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.Log(LogLevel.Information, "Shutting Down");
                _service.Stop();
            }
        }
    }
}