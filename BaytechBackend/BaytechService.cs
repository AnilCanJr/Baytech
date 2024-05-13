using System;
using System.Net.Http;
using System.Text;
using BaytechBackend.DTO_s;
using BaytechBackend.DTOs;
using BaytechBackend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BaytechBackend
{
	public class BaytechService
	{
		private BaytechDbContext _dbContext;
        private UserManager<User> _userManager;
        private SignInManager <User> _SignInManager;
        public BaytechService(BaytechDbContext dbContext,UserManager<User>userManager,SignInManager<User>signInManager)
		{
			_dbContext = dbContext;
            _userManager = userManager;
            _SignInManager = signInManager;

        }

        public async Task SignUp(SignUpDTO dto)
        {

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
            }
        }

        public async Task<UserCookieDTO> SignIn(SignInDTO dto)
        {

            UserCookieDTO cookie=new UserCookieDTO();
            var signInResult = await _SignInManager.PasswordSignInAsync(dto.Username, dto.Password, false, false);
            if(signInResult.Succeeded==true)
            {
                var user =await _userManager.FindByNameAsync(dto.Username);

                 cookie = new UserCookieDTO() {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                    
                };
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


        public List<Object> returnabc (string name)
        {
            List<Object> list = new List<object>();
            var usersWithName = _dbContext.Users.Where(u => u.UserName.Contains(name)).ToList();
            var groupsWithName = _dbContext.Groups.Where(u => u.Name.Contains(name)).ToList();

            list.AddRange(usersWithName);
            list.AddRange(groupsWithName);
            return list;
        }
    }
}

