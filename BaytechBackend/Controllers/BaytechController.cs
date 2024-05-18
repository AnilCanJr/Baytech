using System;
using BaytechBackend.DTO_s;
using BaytechBackend.DTOs;
using BaytechBackend.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace BaytechBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaytechController:ControllerBase
	{
        public CookieOptions _options; 
        private BaytechService _baytechService;
        public BaytechController(BaytechService baytechService)
        {
            _options= new CookieOptions(); 
            _options.HttpOnly = false;
            _options.Secure = false;
            _baytechService = baytechService;
        }
        
        [HttpPost("SignUp")]
        public async Task<ActionResult> SignUp(SignUpDTO dto)
        {

           var result= await _baytechService.SignUp(dto);
            if (result.Id == 0)
            {
                return BadRequest(result);
            }

            return Ok(result);

        }

        [HttpPost("SignIn")]
        public async  Task<ActionResult>  SignIn(SignInDTO dto)
        {
            
            var result =await _baytechService.SignIn(dto);
            if(result.Id==0)
            {
                return BadRequest(result);
            }

            return Ok(result);
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


        [HttpPost("ReturnGroups")]
        public List<Group> ReturnGroups(IdDTO id)
        {
            return _baytechService.ReturnGroups(id.Id);

        }

        [HttpPost("Gemini")]
        public async Task<string> Gemini(GeminiDTO message)
        {
            return await _baytechService.GeminiAsync(message.Message);
        }



        [HttpPost("returnabc")]
        public  List<Object> returnabc(SearchDTO message)
        {
            return  _baytechService.returnabc(message.Search);
        }



        [HttpPost("exit")]
        public void Exit(IdDTO Id)
        {
             _baytechService.Exit(Id.Id);
        }

        [HttpPost("ChangeName")]
        public void ChangeName(ChangeNameDTO dto)
        {
            _baytechService.ChangeName(dto);
        }

    }
}

 