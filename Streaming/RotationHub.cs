using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Streaming
{
    public class RotationHub : Hub
    {
        private readonly ILogger<RotationHub> _logger;

        public RotationHub(ILogger<RotationHub> logger)
        {
            _logger = logger;
        }

        public async void Rotate(double x, double y, double z)
        {
            await Clients.Others.SendCoreAsync("rotated", new object[] { x, y, z });
        }
    }
}
