using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WebWithLongRunningTaskDashboard
{
    public class ProcessHub : Hub
    {
        public ProcessHub(InboxQueue queue)
        {
            Queue = queue;
        }

        public InboxQueue Queue { get; }

        public async Task StartProcess()
        {
            await Queue.EnqueueRequest(new StartProcessRequest
            {
                ConnectionId = Context.ConnectionId
            });
        }

        public async Task UpdateDashboard(string connectionId, string step)
        {
            await Clients.Client(connectionId).SendAsync("dashboardUpdated", step);
        }
    }
}