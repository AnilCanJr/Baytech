using System;
using BaytechBackend.DTO_s;
using BaytechBackend.DTOs;
using BaytechBackend.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BaytechBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaytechController:ControllerBase
	{
        private BaytechService _baytechService;
        public BaytechController(BaytechService baytechService)
        {
            _baytechService = baytechService;
        }
        
        [HttpPost("SignUp")]
        public async Task SignUp(SignUpDTO dto)
        {
            await _baytechService.SignUp(dto);
        }

        [HttpPost("SignIn")]
        public async Task<Microsoft.AspNetCore.Identity.SignInResult> SignIn(SignInDTO dto)
        {
             var resultr =await _baytechService.SignIn(dto);
            return resultr;
        }


        [HttpPost("ChangePreferences")]
        public void ChangePreferences(PreferenceDTO dto)
        {
             _baytechService.ChangePrefrences(dto);
            
        }

        [HttpPost("AddNatifications")]
        public void AddNatifications(NotificationDTO dto)
        {
            _baytechService.AddNatifications(dto);

        }


        //[HttpPost("AcceptRequestNotification")]
        //public void AcceptRequestNotification([FromBody] int notificationId)
        //{
        //    _baytechService.AcceptRequestNotification(notificationId);

        //}

       
        
        [HttpPost("ReturnFriends")]
        public List<User> ReturnFriends(IdDTO id)
        {
          return _baytechService.ReturnFriends(id);

        }


    }
}

 