using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebWithLongRunningTaskDashboard
{
    public class Worker : BackgroundService
    {
        public Worker(InboxQueue queue, ILogger<Worker> logger)
        {
            Queue = queue;
            Logger = logger;
        }

        public InboxQueue Queue { get; }
        public ILogger<Worker> Logger { get; }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                var request = Queue.DequeueRequest();
                if(request != null)
                {
                    Logger.LogInformation($"Request received from {request.ConnectionId}");
                }

                await Task.Delay(1000);
            }
        }
    }
}