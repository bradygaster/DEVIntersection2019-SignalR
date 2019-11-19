using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebWithLongRunningTaskDashboard
{
    [Route("[controller]")]
    public class StartProcessController : Controller
    {
        public StartProcessController(InboxQueue queue,
            ILogger<StartProcessController> logger)
        {
            Queue = queue;
            Logger = logger;
        }

        public InboxQueue Queue { get; }
        public ILogger<StartProcessController> Logger { get; }

        [HttpGet]
        [Route("{connectionId}")]
        public async Task<ActionResult> Get([FromRoute] string connectionId)
        {
            Logger.LogInformation($"Request received from {connectionId}");
            await Queue.EnqueueRequest(new StartProcessRequest { ConnectionId = connectionId });
            return Ok();
        }
    }
}