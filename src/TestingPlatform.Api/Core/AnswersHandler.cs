using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Api.Models;
using TestingPlatform.Api.Models.Dal;
using EntityState = TestingPlatform.Api.Models.Enums.EntityState;

namespace TestingPlatform.Api.Core
{
    public class AnswersHandler
    {
        private readonly ModelsDataContext _context;

        public AnswersHandler(ModelsDataContext context)
        {
            _context = context;
        }

        public async Task<TestDbo> GetTestAsync(List<AnswerDbo> answersDbo)
        {
            var test = (await _context.Questions
                .Include(q => q.Test)
                .ThenInclude(t => t.Questions)
                .FirstOrDefaultAsync(q => q.Id == answersDbo[0].QuestionId))?.Test;

            if (test == null 
                || test.State == EntityState.Deleted
                || answersDbo.Count != answersDbo.Select(a => a.QuestionId).Distinct().Count())
                return null;

            var questionsIds = test.Questions.Select(q => q.Id).ToList();
            return answersDbo.Any(a => !questionsIds.Contains(a.QuestionId)) ? null : test;
        }

        public async Task<Guid> SaveAnswersAsync(List<AnswerDbo> answersDbo, TestDbo test, UserDbo user)
        {
            await SetRightAnswers(answersDbo);
            var score = CalculateScore(answersDbo, test);
            var result = await SaveResult(test, user, score);
            await SaveAnswersWithResult(answersDbo, result);
            
            return result.Id;
        }

        private async Task SetRightAnswers(List<AnswerDbo> answersDbo)
        {
            foreach (var answerDbo in answersDbo)
            {
                var question = await _context.Questions.SingleAsync(q => q.Id == answerDbo.QuestionId);
                answerDbo.RightAnswer = question.Answer;
            }
        }

        private double CalculateScore(List<AnswerDbo> answersDbo, TestDbo testDbo)
        {
            var rightAnswersCount = answersDbo.Count(a => 
                string.Equals(a.RightAnswer, a.UserAnswer, StringComparison.InvariantCultureIgnoreCase));

            return (double) rightAnswersCount / testDbo.Questions.Count;
        }

        private async Task<ResultDbo> SaveResult(TestDbo test, UserDbo user, double score)
        {
            var result = new ResultDbo
            {
                User = user,
                DateTime = DateTime.Now,
                Test = test,
                Score = score
            };
            _context.Add(result);
            await _context.SaveChangesAsync();

            return result;
        }

        private async Task SaveAnswersWithResult(List<AnswerDbo> answersDbo, ResultDbo resultDbo)
        {
            foreach (var answerDbo in answersDbo)
            {
                answerDbo.Result = resultDbo;
                _context.Add(answerDbo);
            }
            await _context.SaveChangesAsync();
        }
    }
}