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
    [Route("api/v1/Answers")]
    public class AnswersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ModelsDataContext _context;

        public AnswersController(IMapper mapper, ModelsDataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpPost("SendList")]
        public async Task<ActionResult> SendListAsync([FromBody] List<AnswerDto> answersDto)
        {
            var rightAnswersCount = 0;
            var questions = new List<QuestionDbo>();
            var test = (await _context.Questions
                .Include(q => q.Test)
                .SingleAsync(q => q.Id == answersDto[0].QuestionId)).Test;
            foreach (var answerDto in answersDto)
            {
                var question = await _context.Questions.SingleAsync(q => q.Id == answerDto.QuestionId);
                questions.Add(question);
                
                if (string.Equals(question.Answer, answerDto.UserAnswer, StringComparison.InvariantCultureIgnoreCase))
                    rightAnswersCount++;
                
                if (question.Test.Id != test.Id)
                    return BadRequest("The number of tests is more than one");
            }
            
            var user = await GetUser();

            var result = new ResultDbo
            {
                User = user,
                DateTime = DateTime.Now,
                Test = test,
                Score = (double) rightAnswersCount / test.Questions.Count
            };
            _context.Add(result);
            await _context.SaveChangesAsync();
                
            var answersDbo = _mapper.Map<List<AnswerDbo>>(answersDto);
            foreach (var answerDbo in answersDbo)
            {
                answerDbo.RightAnswer = questions.Single(q => q.Id == answerDbo.QuestionId).Answer;
                answerDbo.Result = result;
                _context.Add(answerDbo);
            }
            await _context.SaveChangesAsync();
            
            return Ok(result.Id);
        }

        [HttpGet("List")]
        public async Task<ActionResult> GetListAsync(Guid resultId)
        {
            var result = await _context.Results.Include(r => r.Answers).SingleAsync(r => r.Id == resultId);
            var answers = _mapper.Map<List<AnswerDto>>(result.Answers);
            
            return Ok(answers);
        }
        
        private async Task<UserDbo> GetUser() => 
            await _context.Users.SingleAsync(u => u.Id == GetUserId());

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}