using Microsoft.AspNetCore.SignalR;

namespace SignalRDemo.Hubs
{
    public class LiveHub : Hub
    {
        public async Task Join(string user)
        {
            await Clients.All.SendAsync("Joined", user);
        }
    }
}