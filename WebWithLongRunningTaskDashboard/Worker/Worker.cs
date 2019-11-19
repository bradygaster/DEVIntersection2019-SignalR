using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebWithLongRunningTaskDashboard
{
    public class Worker : BackgroundService
    {
        public Worker(
            ProcessHubClient processHubClient,
            InboxQueue queue,
            ILogger<Worker> logger)
        {
            ProcessHubClient = processHubClient;
            Queue = queue;
            Logger = logger;
        }

        public ProcessHubClient ProcessHubClient { get; }
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
                    await ProcessHubClient.UpdateDashboard(request.ConnectionId, "first");
                    await Task.Delay(2000);
                    await ProcessHubClient.UpdateDashboard(request.ConnectionId, "second");
                    await Task.Delay(1000);
                    await ProcessHubClient.UpdateDashboard(request.ConnectionId, "third");
                    await Task.Delay(3000);
                }

                await Task.Delay(1000);
            }
        }
    }
}