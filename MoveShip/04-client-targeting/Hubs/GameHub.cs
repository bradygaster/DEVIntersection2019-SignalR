using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace RealTimeForAll.Hubs
{
    public class GameHub : Hub
    {
        public async Task MoveShip(string y, string x)
        {
            await Clients.Others.SendAsync("shipMoved", y, x);
        }
    }
}