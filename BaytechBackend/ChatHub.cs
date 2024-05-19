using System;
using Microsoft.AspNetCore.SignalR;

namespace BaytechBackend
{
	public class ChatHub:Hub
	{
        //private readonly SharedDB _shared;

        //public ChatHub(SharedDB shared) => _shared = shared;

        //public async Task JoinChat(UserConnection connection)
        //{
        //	await Clients.All.SendAsync("ReceiveMessage", "admin", $"{connection.Username} has joined");
        //}

        //      public async Task JoinSpecificChat(UserConnection connection)
        //      {
        //	await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);

        //	_shared.connections[Context.ConnectionId]=connection;
        //	await Clients.Group(connection.ChatRoom).SendAsync("ReceiveMessage", "admin", $"{connection.Username} has joined { connection.ChatRoom}");
        //      }

        //public async Task SendMessage(string msg)
        //{
        //	if(_shared.connections.TryGetValue(Context.ConnectionId,out UserConnection connection))
        //	{
        //		await Clients.Group(connection.ChatRoom)
        //			.SendAsync("ReceiveSpecificMessage", connection.Username, msg);
        //	}
        //}





        private readonly string _botUser;
        private readonly IDictionary<string, UserConnection> _connections;

        public ChatHub(IDictionary<string, UserConnection> connections)
        {
            _botUser = "MyChat Bot";
            _connections = connections;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has left");
                SendUsersConnected(userConnection.Room);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

            _connections[Context.ConnectionId] = userConnection;

            //await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has joined {userConnection.Room}");

            await SendUsersConnected(userConnection.Room);
        }

        public async Task SendMessage(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", userConnection.User, message);
                
                
            }
        }

        public Task SendUsersConnected(string room)
        {
            var users = _connections.Values
                .Where(c => c.Room == room)
                .Select(c => c.User);
            foreach (var user in users)
            {
                Console.WriteLine(user);
            }
                
            return Clients.Group(room).SendAsync("UsersInRoom", users);
        }


        

    }
}

