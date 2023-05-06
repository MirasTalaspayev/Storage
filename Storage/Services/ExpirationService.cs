namespace Storage.Services
{
    public class ExpirationService : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger<ExpirationService> _logger;

        public ExpirationService(IHostApplicationLifetime appLifetime, ILogger<ExpirationService> logger)
        {
            _appLifetime = appLifetime;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ExpirationService is starting.");

            // Schedule the expiration check to run every minute
            var interval = TimeSpan.FromMinutes(1);
            var timer = new Timer(ExpirationCheck, null, interval, interval);

            _appLifetime.ApplicationStopping.Register(() => {
                _logger.LogInformation("ExpirationService is stopping.");
                timer?.Change(Timeout.Infinite, 0);
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ExpirationService is stopping.");

            return Task.CompletedTask;
        }

        private void ExpirationCheck(object state)
        {
            _logger.LogInformation("Running expiration check...");

            // Get the current time
            var now = DateTime.UtcNow;

            // Iterate through each item in the FileStorage dictionary
            foreach (var item in StorageService.FileStorage)
            {
                // Calculate the expiration time based on the creation time and expiration duration
                var expirationTime = item.Value.CreationTimeUtc.AddMilliseconds(item.Value.ExpiresIn);

                // Check if the item has expired
                if (now > expirationTime)
                {
                    // Remove the item from the dictionary
                    StorageService.FileStorage.Remove(item.Key);

                    _logger.LogInformation($"File with ID {item.Value.FileId} has expired and has been removed from storage.");
                }
            }
        }
    }

}
