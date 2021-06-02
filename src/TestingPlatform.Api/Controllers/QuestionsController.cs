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

namespace TestingPlatform.Api.Controllers
{
    [Authorize]
    [Route("api/v1/Questions")]
    public class QuestionsController : CustomControllerBase
    {
        public QuestionsController(IMapper mapper, ModelsDataContext modelsContext)
            : base(mapper, modelsContext)
        {
        }

        [HttpGet("List")]
        public async Task<ActionResult> GetListAsync(Guid testId)
        {
            var questions = await ModelsContext.Questions.Where(q => q.Test.Id == testId).ToListAsync();
            return Ok(Mapper.Map<List<QuestionDto>>(questions));
        }

        [HttpGet("AddEmpty")]
        public async Task<ActionResult> AddAsync(Guid testId)
        {
            var user = await GetUser();
            var test = await ModelsContext.Tests.FirstOrDefaultAsync(t => t.Id == testId);
            if (test == null)
                return BadRequest("Test does not exist");

            if (test.Owner?.Id != user.Id)
                return Forbid();

            var question = new QuestionDbo { Test = test };
            ModelsContext.Add(question);
            await ModelsContext.SaveChangesAsync();

            return Ok(question.Id);
        }

        [HttpPatch]
        public async Task<ActionResult> UpdateAsync([FromBody] QuestionDto questionDto)
        {
            var questionDbo = await ModelsContext.Questions.Include(q => q.Test).FirstOrDefaultAsync(q => q.Id == questionDto.Id);
            if (questionDbo == null)
                return BadRequest("Question does not exist");

            var user = await GetUser();
            if (questionDbo.Test.Owner?.Id != user.Id)
                return Forbid();

            questionDbo.Question = questionDto.Question;
            questionDbo.Answer = questionDto.Answer;
            ModelsContext.Update(questionDbo);
            await ModelsContext.SaveChangesAsync();

            return Ok();
        }
        
        [HttpDelete]
        public async Task<ActionResult> RemoveAsync(Guid questionId)
        {
            var questionDbo = await ModelsContext.Questions.Include(q => q.Test).FirstOrDefaultAsync(q => q.Id == questionId);
            if (questionDbo == null)
                return BadRequest("Question does not exist");

            var user = await GetUser();
            if (questionDbo.Test.Owner?.Id != user.Id)
                return Forbid();

            ModelsContext.Remove(questionDbo);
            await ModelsContext.SaveChangesAsync();
            
            return Ok();
        }
    }
}