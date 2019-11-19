using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebWithLongRunningTaskDashboard
{
    public class ProcessHubClient
    {
        internal const string HUB_CLIENT_URL_CONFIG = "HubProcessUrl";

        public ProcessHubClient(IConfiguration configuration,
            ILogger<ProcessHubClient> logger)
        {
            Configuration = configuration;
            Logger = logger;
            HubUrl = Configuration[HUB_CLIENT_URL_CONFIG];
            Connection = new HubConnectionBuilder()
                .WithUrl(HubUrl)
                .Build();
        }

        public IConfiguration Configuration { get; }
        public ILogger<ProcessHubClient> Logger { get; }
        public string HubUrl { get; private set; }
        public HubConnection Connection { get; private set; }

        public async Task UpdateDashboard(string connectionId,
            string stepCompleted)
        {
            Logger.LogInformation($"Updating dashboard that {connectionId} completed step {stepCompleted}");
            if(Connection.State != HubConnectionState.Connected)
            {
                Logger.LogInformation($"Connecting to hub at {HubUrl}.");
                await Connection.StartAsync();
                Logger.LogInformation($"Connected to hub.");
            }

            await Connection.SendAsync("UpdateDashboard", connectionId, stepCompleted);
            Logger.LogInformation($"Updated dashboard.");
        }
    }

    public static class ProcessHubClientExtensions
    {
        public static IServiceCollection AddProcessHubClient(
            this IServiceCollection services
        )
        {
            services.AddSingleton<ProcessHubClient>();
            return services;
        }
    }
}