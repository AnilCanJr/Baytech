using System;
using System.Collections.Concurrent;
using BaytechBackend.Entities;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BaytechBackend
{
    public class ChatHubb : Hub
    {
        private readonly BaytechDbContext _context;
        static ConcurrentDictionary<string, string> ConnectedUsers = new ConcurrentDictionary<string, string>();

        public ChatHubb(BaytechDbContext context)
        {
            _context = context;
        }

        public override Task OnConnectedAsync()
        {

            var connectionId = Context.ConnectionId;
            var username = Context.GetHttpContext().Request.Query["username"];
            

           

            ConnectedUsers.TryAdd(Context.ConnectionId, username);


            var user = _context.Users.FirstOrDefault(x => x.UserName == username);
            if (user != null)
            {
                user.ConnectionId = connectionId;
                _context.Update(user);
                _context.SaveChanges();
            }

            return base.OnConnectedAsync();
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            string? username;

            ConnectedUsers.TryRemove(Context.ConnectionId, out var userName);

            var user = _context.Users.FirstOrDefault(x => x.UserName == userName);
            if (user != null)
            {
                user.ConnectionId = null;
                _context.Update(user);
                _context.SaveChanges();
            }


            return base.OnDisconnectedAsync(exception);
        }   

     

        //private message one to one
        public async Task SendPrivateMessage(string toUser, String msg)
        {

            
            if (ConnectedUsers.TryGetValue(toUser, out var connectionId))
            {
                var senderUsername = ConnectedUsers.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;

                var newMessage = new Chat
                {
                    SenderUsername = senderUsername,
                    ReceiverUsername = toUser,
                    Message = msg,
                    Timestamp = DateTime.Now
                };

                _context.Chats.Add(newMessage);
                await _context.SaveChangesAsync();
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", msg);
                await Clients.Client(connectionId).SendAsync("Notify", "You have a new message.");

            }


        }

        public async Task CreateGroup(string groupName)
        {
            var group = new Group
            {
                Name = groupName,
                GroupUsers = new List<GroupUser>()
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
        }

        public async Task AddUserToGroup(string groupName, string username)
        {
            var group = _context.Groups.FirstOrDefault(g => g.Name == groupName);
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);

            if (group != null && user != null)
            {
                var groupUser = new GroupUser
                {
                    GroupId = group.Id,
                    UserId = user.Id
                };

                _context.GroupUsers.Add(groupUser);
                await _context.SaveChangesAsync();

                await Groups.AddToGroupAsync(ConnectedUsers[username], groupName);
            }
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {
            var senderUsername = ConnectedUsers.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;

            var groupMessage = new GroupMessage
            {
                GroupId = _context.Groups.FirstOrDefault(g => g.Name == groupName).Id,
                SenderUsername = senderUsername,
                Message = message,
                Timestamp = DateTime.Now
            };

            _context.GroupMessages.Add(groupMessage);
            await _context.SaveChangesAsync();

            await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", message);
            await Clients.Group(groupName).SendAsync("Notify", $"New message in group {groupName}.");
        }
        

    }
}

