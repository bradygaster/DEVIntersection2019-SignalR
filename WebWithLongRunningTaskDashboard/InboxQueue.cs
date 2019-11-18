using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebWithLongRunningTaskDashboard
{
    public class InboxQueue
    {
        const string QUEUE_CONFIG_CONNECTION = "Azure:Storage:Incoming:ConnectionString";
        const string QUEUE_CONFIG_KEY = "Azure:Storage:Incoming:QueueName";

        public InboxQueue(IConfiguration configuration, ILogger<InboxQueue> logger)
        {
            Configuration = configuration;
            Logger = logger;

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(Configuration[QUEUE_CONFIG_CONNECTION],
                out _storageAccount))
            {
                var queueClient = _storageAccount.CreateCloudQueueClient();
                AzureQueue = queueClient.GetQueueReference(Configuration[QUEUE_CONFIG_KEY]);
                AzureQueue.CreateIfNotExists();
            }
            else
            {
                Logger.LogWarning(
                    "A connection string has not been defined in the system environment variables. " +
                    "Add an environment variable named 'storageconnectionstring' with your storage " +
                    "connection string as a value.");
            }
        }

        private CloudStorageAccount _storageAccount;
        private IConfiguration Configuration { get; }
        public ILogger<InboxQueue> Logger { get; }
        private CloudQueue AzureQueue { get; }
        public StartProcessRequest DequeueRequest()
        {
            var cloudQueueMessage = AzureQueue.GetMessage();
            if (cloudQueueMessage == null) return null;

            var result = JsonSerializer.Deserialize<StartProcessRequest>(cloudQueueMessage.AsString);
            AzureQueue.DeleteMessage(cloudQueueMessage);

            return result;
        }

        public async Task<bool> EnqueueRequest(StartProcessRequest result)
        {
            try
            {
                var json = JsonSerializer.Serialize<StartProcessRequest>(result);
                await AzureQueue.AddMessageAsync(new CloudQueueMessage(json, false));
                Logger.LogInformation("Request enqueued");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public static class InboxQueueExtensions
    {
        public static IServiceCollection AddInboxQueueSupport(this IServiceCollection services)
        {
            services.AddSingleton<InboxQueue>();
            return services;
        }
    }
}
