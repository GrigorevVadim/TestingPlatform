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
    [Route("api/v1/Questions")]
    public class QuestionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ModelsDataContext _context;

        public QuestionsController(IMapper mapper, ModelsDataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("List")]
        public async Task<ActionResult> GetListAsync(Guid testId)
        {
            var questions = await _context.Questions.Where(q => q.Test.Id == testId).ToListAsync();
            return Ok(_mapper.Map<List<QuestionDto>>(questions));
        }

        [HttpGet("AddEmpty")]
        public async Task<ActionResult> AddAsync(Guid testId)
        {
            var user = await GetUser();
            var test = await _context.Tests.FirstOrDefaultAsync(t => t.Id == testId);
            if (test == null)
                return BadRequest("Test does not exist");

            if (test.Owner?.Id != user.Id)
                return Forbid();

            var question = new QuestionDbo { Test = test };
            _context.Add(question);
            await _context.SaveChangesAsync();

            return Ok(question.Id);
        }

        [HttpPatch]
        public async Task<ActionResult> UpdateAsync([FromBody] QuestionDto questionDto)
        {
            var questionDbo = await _context.Questions.Include(q => q.Test).FirstOrDefaultAsync(q => q.Id == questionDto.Id);
            if (questionDbo == null)
                return BadRequest("Question does not exist");

            var user = await GetUser();
            if (questionDbo.Test.Owner?.Id != user.Id)
                return Forbid();

            questionDbo.Question = questionDto.Question;
            questionDbo.Answer = questionDto.Answer;
            _context.Update(questionDbo);
            await _context.SaveChangesAsync();

            return Ok();
        }
        
        [HttpDelete]
        public async Task<ActionResult> RemoveAsync(Guid questionId)
        {
            var questionDbo = await _context.Questions.Include(q => q.Test).FirstOrDefaultAsync(q => q.Id == questionId);
            if (questionDbo == null)
                return BadRequest("Question does not exist");

            var user = await GetUser();
            if (questionDbo.Test.Owner?.Id != user.Id)
                return Forbid();

            _context.Remove(questionDbo);
            await _context.SaveChangesAsync();
            
            return Ok();
        }

        private async Task<UserDbo> GetUser() => 
            await _context.Users.SingleAsync(u => u.Id == GetUserId());

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}