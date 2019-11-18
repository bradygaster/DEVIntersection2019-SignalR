using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebWithLongRunningTaskDashboard
{
    [Route("[controller]")]
    public class StartProcessController : Controller
    {
        public StartProcessController(InboxQueue queue)
        {
            Queue = queue;
        }

        public InboxQueue Queue { get; }

        [HttpGet]
        public async Task<ActionResult> Get(string connectionId)
        {
            await Queue.EnqueueRequest(new StartProcessRequest { ConnectionId = connectionId });
            return Ok();
        }
    }

    public class StartProcessRequest
    {
        public string ConnectionId { get; set; }
    }
}