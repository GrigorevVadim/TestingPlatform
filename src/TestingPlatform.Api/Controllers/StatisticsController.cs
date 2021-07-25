using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Api.Core;
using TestingPlatform.Api.Helpers;
using TestingPlatform.Api.Models;
using TestingPlatform.Api.Models.Dto;

namespace TestingPlatform.Api.Controllers
{
    [Authorize]
    [Route("api/v1/Statistics")]
    public class StatisticsController : CustomControllerBase
    {
        public StatisticsController(IMapper mapper, ModelsDataContext modelsContext)
            : base(mapper, modelsContext)
        {
        }

        [HttpGet("GetResultList")]
        public async Task<ActionResult> GetResultListAsync(Guid testId)
        {
            var results = await ModelsContext.Results
                .Include(r => r.User)
                .Where(r => r.Test.Id == testId)
                .ToListAsync();
            
            return Ok(Mapper.Map<List<ResultDto>>(results));
        }

        [HttpGet("GetAverageScore")]
        public async Task<ActionResult> GetAverageScoreAsync(Guid testId)
        {
            var results = await ModelsContext.Results
                .Where(r => r.Test.Id == testId)
                .ToListAsync();

            var average = results.Sum(result => result.Score) / results.Count;

            return Ok(average);
        }

        [HttpGet("GetScorePerQuestions")]
        public async Task<ActionResult> GetScorePerQuestionsAsync(Guid testId)
        {
            var answers = await ModelsContext.Results
                .Include(r => r.Answers)
                .ThenInclude(a => a.Question)
                .Where(r => r.Test.Id == testId)
                .SelectMany(r => r.Answers)
                .ToListAsync();

            var questionScores = answers.GroupBy(a => a.Question.Question)
                .Select(g => new
                {
                    Question = g.Key, 
                    Score = (double) g.Count(a => 
                        AnswersHelper.CheckAnswer(a.UserAnswer, a.RightAnswer)) / g.Count()
                })
                .ToList();

            return Ok(questionScores);
        }

        [HttpGet("GetScoreDistribution")]
        public async Task<ActionResult> GetScoreDistributionAsync(Guid testId)
        {
            var results = await ModelsContext.Results
                .Where(r => r.Test.Id == testId)
                .ToListAsync();

            var list = new List<double>
            {
                (double) results.Count(r => r.Score < 0.25) / results.Count,
                (double) results.Count(r => r.Score is >= 0.25 and < 0.5) / results.Count,
                (double) results.Count(r => r.Score is >= 0.5 and < 0.75) / results.Count,
                (double) results.Count(r => r.Score >= 0.75) / results.Count
            };

            return Ok(list);
        }
    }
}