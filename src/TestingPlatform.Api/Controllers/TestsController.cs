using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Api.Core;
using TestingPlatform.Api.Models;
using TestingPlatform.Api.Models.Dal;
using TestingPlatform.Api.Models.Dto;
using EntityState = TestingPlatform.Api.Models.Enums.EntityState;

namespace TestingPlatform.Api.Controllers
{
    [Authorize]
    [Route("api/v1/Tests")]
    public class TestsController : CustomControllerBase
    {
        public TestsController(IMapper mapper, ModelsDataContext modelsContext)
            : base(mapper, modelsContext)
        {
        }

        [HttpGet("List")]
        public async Task<ActionResult> GetListAsync()
        {
            var list = await ModelsContext.Tests
                .Where(t => t.Owner.Id == GetUserId() && t.State != EntityState.Deleted)
                .ToListAsync();
            return Ok(Mapper.Map<List<TestDto>>(list));
        }

        [HttpGet("AddEmpty")]
        public async Task<ActionResult> AddAsync()
        {
            var user = await GetUser();
            var test = new TestDbo { Owner = user };
            ModelsContext.Add(test);
            await ModelsContext.SaveChangesAsync();
            
            return Ok(test.Id);
        }

        [HttpPatch]
        public async Task<ActionResult> UpdateAsync([FromBody] TestDto testDto)
        {
            var user = await GetUser();
            var test = await ModelsContext.Tests
                .FirstOrDefaultAsync(t => t.Id == testDto.Id && t.State != EntityState.Deleted);

            if (test == null)
                return BadRequest("Test not exists");

            if (test.Owner?.Id != user.Id)
                return Forbid();

            test.Name = testDto.Name;
            ModelsContext.Update(test);
            await ModelsContext.SaveChangesAsync();
            
            return Ok();
        }
        
        [HttpGet]
        public async Task<ActionResult> GetAsync(Guid testId)
        {
            var test = await ModelsContext.Tests
                .FirstOrDefaultAsync(t => t.Id == testId && t.State != EntityState.Deleted);
            if (test == null)
                return BadRequest("Test does not exist");

            return Ok(Mapper.Map<TestDto>(test));
        }
        
        [HttpDelete]
        public async Task<ActionResult> RemoveAsync(Guid testId)
        {
            var user = await GetUser();
            var test = await ModelsContext.Tests.FirstOrDefaultAsync(t => t.Id == testId && t.State != EntityState.Deleted);
            if (test == null)
                return BadRequest("Test does not exist");

            if (test.Owner?.Id != user.Id)
                return Forbid();

            test.State = EntityState.Deleted;
            ModelsContext.Update(test);
            await ModelsContext.SaveChangesAsync();
            
            return Ok();
        }
    }
}