using CHNUCooin.Database;

namespace CHNUCooin.Services.Background
{
    public class ProcessTransactionBackgroundService(ApplicationContext applicationContext) : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly TransactionProcessingService _transactionProcessingService = new(applicationContext);
        private Timer? _timer = null;

        public Task StartAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(15));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            var count = Interlocked.Increment(ref executionCount);
            _transactionProcessingService.ProcessTransactions();

            Console.WriteLine(
                "Timed Hosted Service is working. Count: {Count}" + count);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}