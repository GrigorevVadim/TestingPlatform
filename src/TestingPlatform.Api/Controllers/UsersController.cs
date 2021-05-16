using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestingPlatform.Api.Authentication;
using TestingPlatform.Api.Models;
using TestingPlatform.Api.Models.Dal;
using TestingPlatform.Api.Models.Dto;

namespace TestingPlatform.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ModelsDataContext _context;

        public UsersController(IMapper mapper, ModelsDataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUserAsync([FromBody] UserDto userDto)
        {
            var userDbo = _mapper.Map<UserDbo>(userDto);
            userDbo.Token = Guid.NewGuid();
            userDbo.LastLogIn = DateTime.Now;
            _context.Add(userDbo);
            await _context.SaveChangesAsync();

            Response.Cookies.Append(AuthConstants.TokenCookie, userDbo.Token.ToString());
            return Ok(userDbo.Token);
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginAsync([FromBody] UserDto userDto)
        {
            var userDbo = _context.Users.FirstOrDefault(u => u.Login == userDto.Login && u.Password == userDto.Password);
            if (userDbo == null)
                return Forbid();

            userDbo.Token = Guid.NewGuid();
            userDbo.LastLogIn = DateTime.Now;
            _context.Update(userDbo);
            await _context.SaveChangesAsync();

            Response.Cookies.Append(AuthConstants.TokenCookie, userDbo.Token.ToString());
            return Ok(userDbo.Token);
        }

        [HttpGet("logout")]
        public ActionResult Logout()
        {
            Response.Cookies.Delete(AuthConstants.TokenCookie);
            return Ok();
        }
    }
}