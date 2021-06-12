using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestingPlatform.Api.Authentication;
using TestingPlatform.Api.Core;
using TestingPlatform.Api.Models;
using TestingPlatform.Api.Models.Dal;
using TestingPlatform.Api.Models.Dto;

namespace TestingPlatform.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/users")]
    public class UsersController : CustomControllerBase
    {
        public UsersController(IMapper mapper, ModelsDataContext modelsContext)
            : base(mapper, modelsContext)
        {
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUserAsync([FromBody] UserDto userDto)
        {
            var userDbo = Mapper.Map<UserDbo>(userDto);
            var count = ModelsContext.Users.Count(u => u.Login == userDto.Login);
            if (count != 0)
                return BadRequest("User already exist");
            
            userDbo.Token = Guid.NewGuid();
            userDbo.LastLogIn = DateTime.Now;
            ModelsContext.Add(userDbo);
            await ModelsContext.SaveChangesAsync();

            Response.Cookies.Append(AuthConstants.TokenCookie, userDbo.Token.ToString());
            return Ok(userDbo.Token);
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginAsync([FromBody] UserDto userDto)
        {
            var userDbo = ModelsContext.Users.FirstOrDefault(u => u.Login == userDto.Login && u.Password == userDto.Password);
            if (userDbo == null)
                return Forbid();

            userDbo.Token = Guid.NewGuid();
            userDbo.LastLogIn = DateTime.Now;
            ModelsContext.Update(userDbo);
            await ModelsContext.SaveChangesAsync();

            Response.Cookies.Append(AuthConstants.TokenCookie, userDbo.Token.ToString());
            return Ok(userDbo.Token);
        }

        [HttpGet("logout")]
        public async Task<ActionResult> LogoutAsync()
        {
            await Task.Run(() => Response.Cookies.Delete(AuthConstants.TokenCookie));
            return Ok();
        }
    }
}