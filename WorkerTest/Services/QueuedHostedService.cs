using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkerTest.Services
{
    #region snippet1
    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private IBackgroundTaskQueue TaskQueue;
        //private readonly ITestService testService;
        private readonly Func<string, ITestService> testService;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue, 
            ILogger<QueuedHostedService> logger, Func<string, ITestService> testService)
        {
            TaskQueue = taskQueue;
            _logger = logger;
            this.testService = testService;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            for (var i = 0; i < 100; i++)
            {
                var j = i;
                var random = new Random();
                var service = "Servis1";
                if (i <= random.Next(1, 100))
                    service = "";
                TaskQueue.QueueBackgroundWorkItem(async token =>
                {
                    //var random = new Random();
                    //await Task.Delay(random.Next(50, 1000), token);
                    await this.testService(service).DoSomething(token);
                    _logger.LogInformation($"Event {j} Service {service}");

                });
            }

            var semaphore = new SemaphoreSlim(7);

            void HandleTask(Task task)
            {
                semaphore.Release();
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                await semaphore.WaitAsync();
                var item = await TaskQueue.DequeueAsync(stoppingToken);

                var task = item(stoppingToken);
                task.ContinueWith(HandleTask);
            }
        }
    }
    #endregion
}
