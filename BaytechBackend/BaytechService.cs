using System;
using System.Net.Http;
using System.Text;
using BaytechBackend.DTO_s;
using BaytechBackend.DTOs;
using BaytechBackend.Entities;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace BaytechBackend
{
	public class BaytechService
	{
		private BaytechDbContext _dbContext;
        private UserManager<User> _userManager;
        private SignInManager <User> _SignInManager;

        private IHubContext<ChatHubb> _hubContext;

        public BaytechService(BaytechDbContext dbContext,UserManager<User>userManager,SignInManager<User>signInManager, IHubContext<ChatHubb> hubContext)
		{
			_dbContext = dbContext;
            _userManager = userManager;
            _SignInManager = signInManager;
            _hubContext = hubContext;
        }

        public async Task<UserCookieDTO> SignUp(SignUpDTO dto)
        {
            UserCookieDTO cookie = new UserCookieDTO();
            User newUser = new()
            {
                UserName = dto.Username,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                IsOnline = true

            };

            if(await  _userManager.CreateAsync(newUser, dto.Password)==IdentityResult.Success)
            {
                var user = _dbContext.Users.Where(x => x.UserName == dto.Username).SingleOrDefault();

                _dbContext.Prefernces.Add(
                    new Prefernce()
                    {
                        User = user
                    }
                    );
                _dbContext.SaveChanges();

                cookie = new UserCookieDTO()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email

                };
                return cookie;
            }
            else
            {
                return cookie;
            }
         
        }

        public async Task<UserCookieDTO> SignIn(SignInDTO dto)
        {

            UserCookieDTO cookie = new UserCookieDTO();
            var signInResult = await _SignInManager.PasswordSignInAsync(dto.Username, dto.Password, false, false);
            if (signInResult.Succeeded == true)
            {
                var user = await _userManager.FindByNameAsync(dto.Username);
                user.IsOnline = true;
                _dbContext.SaveChanges();

                cookie = new UserCookieDTO()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email

                };
                return cookie;
            }
            return cookie;
        }
            

        public void ChangePrefrences(PreferenceDTO dto)
        {

            var prefence= _dbContext.Prefernces.Where(x => x.UserId == dto.UserId).FirstOrDefault();
            prefence.DarkMode = dto.DarkMode;
            prefence.LastSeenOn = dto.LastSeenOn;
            prefence.PrivateProfile = dto.PrivateProfile;
            _dbContext.SaveChanges();

        }

        public void AddFriend(FriendRequestDTO dto)
        {
            var friend = new Friend()
            {
                UserOneId = dto.UserOneId,
                UserTwoId = dto.UserTwoId
            };

            _dbContext.Friends.Add(friend);
            _dbContext.SaveChanges();

            //send signalR notification
            AddNatifications(new NotificationDTO
            {
                ClientUserId = dto.UserOneId,
                TargetUserId = dto.UserTwoId,
                NotificationTypeId = 1
            });

            var user = _dbContext.Users.FirstOrDefault(z => z.Id == dto.UserTwoId);
            if (user != null && !string.IsNullOrEmpty(user.ConnectionId))
            {
                 _hubContext.Clients.Client(user.ConnectionId).SendAsync("Notify", "You have a new friend request");
            }


        }

        public void RemoveFriend(FriendRequestDTO dto)
        {
             var friend = _dbContext.Friends.FirstOrDefault(x => x.UserOneId == dto.UserOneId && x.UserTwoId == dto.UserTwoId);
             if (friend != null) _dbContext.Friends.Remove(friend);
             _dbContext.SaveChanges();
        }



        public List<Friend> GetFriends(IdDTO userId)
        {
            var friends = _dbContext.Friends.Include(z => z.UserOne).Include(z => z.UserTwo).Where(x => x.UserOneId == userId.Id ||  x.UserTwoId == userId.Id).ToList();
            return friends;
        }   

        public void AddNatifications(NotificationDTO dto)
        {
            var notification = new Notification()
            {
                ClientUserId = dto.ClientUserId,
                TargetUserId = dto.TargetUserId,
                NotificationTypeId = dto.NotificationTypeId,
                Done=false
            };

            _dbContext.Notificationes.Add(notification);

            _dbContext.SaveChanges();
        }


        public List<Notification> GetNotifications(IdDTO userId)
        {
            var notifications = _dbContext.Notificationes.Include(z => z.NotificationType).Where(x => x.TargetUserId == userId.Id).ToList();
            return notifications;
        }   

        public void AddFriendship(int notificationId)
        {
            var notification = _dbContext.Notificationes.Where(x => x.Id == notificationId).FirstOrDefault();

            var friend = new Friend()
            {
                UserOneId=notification.ClientUserId,
                UserTwoId=notification.TargetUserId
            };

            _dbContext.Friends.Add(friend);

            _dbContext.SaveChanges();

            
            notification.Done = true;
            _dbContext.SaveChanges();

        }


        public List<User> ReturnFriends(IdDTO userId)
        {
            List<User> users = new List<User>();
            var friends = _dbContext.Friends.Include(x=>x.UserTwo).Where(x => x.UserOneId == userId.Id).ToList();
            foreach(var friend in friends)
            {
                users.Add(friend.UserTwo);
            }
            var friends2 = _dbContext.Friends.Include(x => x.UserOne).Where(x => x.UserTwoId == userId.Id).ToList();
            foreach (var friend in friends2)
            {
                users.Add(friend.UserOne);
            }
            return users;
        }


        public List<Group> ReturnGroups(int userId)
        {
            List<Group> groups = new List<Group>();
            var groupUsers = _dbContext.GroupUsers.Include(x => x.Group).Where(x => x.UserId == userId).ToList();
            foreach (var groupUser in groupUsers)
            {
                groups.Add(groupUser.Group);
            }
         
           
            return groups;
        }


        public int ReturnId(string username)
        {
            return _userManager.FindByNameAsync(username).Id;
        }


        public async Task<string> GeminiAsync(string username)
        {
               HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://generativelanguage.googleapis.com");
            string jsonContent = "{\"contents\":{\"parts\":{\"text\":\"" + username + "\"}}}";
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
             HttpResponseMessage response = await client.PostAsync("v1beta/models/gemini-pro:generateContent?key=AIzaSyAippx48rfTZCxRy1h7AHO1jUCOQQzPf_k",content);

            return await response.Content.ReadAsStringAsync();


            //return _userManager.FindByNameAsync(username).Id;
        }



        public async Task<string> PrivateGemini(GeminiDTO dto)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://generativelanguage.googleapis.com");
            string jsonContent = "{\"contents\":{\"parts\":{\"text\":\"" + dto.Message + "\"}}}";
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("v1beta/models/gemini-pro:generateContent?key=AIzaSyAippx48rfTZCxRy1h7AHO1jUCOQQzPf_k", content);

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

            string text = json["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

            var newMessage = new Chat
            {
                SenderUsername = dto.Username,
                ReceiverUsername = "Gemini",
                Message = dto.Message,
                Timestamp = DateTime.UtcNow

            };
            _dbContext.Chats.Add(newMessage);
            _dbContext.SaveChanges();

            var newMessage1 = new Chat
            {
                SenderUsername = "Gemini",
                ReceiverUsername = dto.Username,
                Message = text,
                Timestamp = DateTime.UtcNow

                

        };
            _dbContext.Chats.Add(newMessage1);
            _dbContext.SaveChanges();



            return await response.Content.ReadAsStringAsync();


            //return _userManager.FindByNameAsync(username).Id;
        }


        public List<Object> returnabc (string name)
        {
            List<Object> list = new List<object>();
            var usersWithName = _dbContext.Users.Where(u => u.UserName.Contains(name)).ToList();
            var groupsWithName = _dbContext.Groups.Where(u => u.Name.Contains(name)).ToList();

            list.AddRange(usersWithName);
            list.AddRange(groupsWithName);
            return list;
        }


        public void Exit(int ıd)
        {
     

            var user= _dbContext.Users.Where(x => x.Id == ıd).FirstOrDefault();
            user.IsOnline = false;
            _dbContext.SaveChanges();
        }



        public void ChangeName(ChangeNameDTO dto)
        {
            var user = _dbContext.Users.Where(x => x.Id == dto.Id).FirstOrDefault();
            user.UserName = dto.Username;
            _dbContext.SaveChanges();
        }

         public List<Chat> GetMessages(MessagesDTO dto)
        {

            var messages = _dbContext.Chats.Where(x => x.SenderUsername == dto.UserOneName).Where(x=>x.ReceiverUsername==dto.UserTwoName).ToList();
            var messages2 = _dbContext.Chats.Where(x => x.SenderUsername == dto.UserTwoName).Where(x => x.ReceiverUsername == dto.UserOneName).ToList();
            messages.AddRange(messages2);
             messages= messages.OrderBy(x=>x.Timestamp).ToList();
            return messages;
        }

    }
}

