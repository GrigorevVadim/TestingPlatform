using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Api.Models;
using TestingPlatform.Api.Models.Dal;
using TestingPlatform.Api.Models.Dto;

namespace TestingPlatform.Api.Controllers
{
    [Authorize]
    [Route("api/v1/Tests")]
    public class TestsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ModelsDataContext _context;

        public TestsController(IMapper mapper, ModelsDataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("List")]
        public async Task<ActionResult> GetListAsync()
        {
            var list = await _context.Tests.Where(t => t.Owner.Id == GetUserId()).ToListAsync();
            return Ok(_mapper.Map<List<TestDto>>(list));
        }

        [HttpGet("AddEmpty")]
        public async Task<ActionResult> AddAsync()
        {
            var user = await GetUser();
            var test = new TestDbo { Owner = user };
            _context.Add(test);
            await _context.SaveChangesAsync();
            
            return Ok(test.Id);
        }

        [HttpPatch]
        public async Task<ActionResult> UpdateAsync([FromBody] TestDto testDto)
        {
            var user = await GetUser();
            var test = await _context.Tests.SingleAsync(t => t.Id == testDto.Id);
            if (test.Owner.Id != user.Id)
                return Forbid();

            test.Name = testDto.Name;
            _context.Update(test);
            await _context.SaveChangesAsync();
            
            return Ok();
        }
        
        [HttpGet]
        public async Task<ActionResult> GetAsync(Guid testId)
        {
            var user = await GetUser();
            var test = await _context.Tests.SingleAsync(t => t.Id == testId);
            if (test.Owner.Id != user.Id)
                return Forbid();
            
            return Ok(_mapper.Map<TestDto>(test));
        }
        
        [HttpDelete]
        public async Task<ActionResult> RemoveAsync(Guid testId)
        {
            var user = await GetUser();
            var test = await _context.Tests.SingleAsync(t => t.Id == testId);
            if (test.Owner.Id != user.Id)
                return Forbid();

            _context.Remove(test);
            await _context.SaveChangesAsync();
            
            return Ok();
        }

        private async Task<UserDbo> GetUser() => 
            await _context.Users.SingleAsync(u => u.Id == GetUserId());

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}