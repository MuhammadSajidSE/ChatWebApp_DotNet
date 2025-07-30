using AuthenticationAuthorization.Context;
using ChatWebApp.DTOLayer;
using ChatWebApp.Models;
using ChatWebApp.ServicesInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly RegistrationInterface registrationInterface;
        private readonly JwtTokenGenerator tokenGenerator;

        public RegistrationController(RegistrationInterface _registrationInterface, JwtTokenGenerator _tokenGenerator)
        {
            registrationInterface = _registrationInterface;
            tokenGenerator = _tokenGenerator;
        }

        [HttpPost("Registartion")]
        public async Task<ActionResult<User>> Register(User user)
        {
            Boolean result = await registrationInterface.CheckExist(user);
            if (result)
            {
                return BadRequest("This Phone number Already exist");
            }
            else
            {
                await registrationInterface.RegisterUser(user);
                return user;
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> LoginUser([FromBody] LoginDTO logindata)
        {
            var userData = await registrationInterface.VerifyUserAsync(logindata);

            if (userData == null)
            {
                return Unauthorized("Incorrect username or password");
            }

            string token = registrationInterface.GenerateJwtToken(userData, tokenGenerator);

            var sessionData = new Session
            {
                JWTtooken = token,
                Generatedtime = DateTime.UtcNow,
                UserId = userData.UserId,
                User = userData
            };

            var newToken = await registrationInterface.SessionUpdate(sessionData);

            return Ok(newToken);
        }

        //[HttpPost("Login")]
        //public async Task<ActionResult<String>> LoginUser(LoginDTO logindata)
        //{
        //    var userData = await registrationInterface.VerifyUserAsync(logindata);
        //    if (userData == null)
        //    {
        //        return NotFound("The credential wrong");
        //    }
        //    else
        //    {
        //        string token = registrationInterface.GenerateJwtToken(userData, tokenGenerator);
        //        var sessionData = new Session
        //        {
        //            JWTtooken = token,
        //            Generatedtime = DateTime.UtcNow,
        //            UserId = userData.UserId,
        //            User = userData
        //        };
        //        var newtooken = await registrationInterface.SessionUpdate(sessionData);
        //        return Ok(newtooken);
        //    }
        //}

        [HttpPost("LoginByTooken")]
        public async Task<ActionResult<User>> LoginByTooken(string tooken)
        {
            var validity = await registrationInterface.CheckTookenValidity(tooken);
            if (validity==false)
            {
                return Unauthorized("The tooken is invalid");
            }
            var data = await registrationInterface.GetByTooken(tooken);
            return data;
        }
    }
}
