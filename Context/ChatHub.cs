using Microsoft.AspNetCore.SignalR;

namespace ChatWebApp.Hubs
{
    //public class ChatHub : Hub
    //{
    //    public async Task SendMessageToUser(string receiverId, object message)
    //    {
    //        await Clients.User(receiverId).SendAsync("ReceiveMessage", message);
    //    }

    //    // Optional: For connection mapping
    //    public override async Task OnConnectedAsync()
    //    {
    //        await base.OnConnectedAsync();
    //    }
    //}
    public class ChatHub : Hub
    {
        // Dictionary to store mapping of user ID to connection ID
        private static readonly Dictionary<int, string> userConnections = new Dictionary<int, string>();

        public override Task OnConnectedAsync()
        {
            // You can optionally use query strings or context info to identify the user
            return base.OnConnectedAsync();
        }

        public Task Register(int userId)
        {
            userConnections[userId] = Context.ConnectionId;
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var item = userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (item.Key != 0)
            {
                userConnections.Remove(item.Key);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public static string GetConnectionId(int userId)
        {
            return userConnections.TryGetValue(userId, out var connectionId) ? connectionId : null;
        }
    }
}
