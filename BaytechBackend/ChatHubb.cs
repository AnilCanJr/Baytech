using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace BaytechBackend
{
	public class ChatHubb: Hub
    {
          static ConcurrentDictionary<string, string> ConnectedUsers = new ConcurrentDictionary<string, string>();

        public override Task OnConnectedAsync()
        {
            var userName = Context.GetHttpContext().Request.Query["username"];
       
                ConnectedUsers.TryAdd(Context.ConnectionId, userName);
            
            return base.OnConnectedAsync();
        }

        public IEnumerable<string> GetConnectedUsers()
        {
            return ConnectedUsers.Values;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            foreach(var abc in GetConnectedUsers())
            {
                Console.WriteLine(abc);
            }
        }
    }
}

