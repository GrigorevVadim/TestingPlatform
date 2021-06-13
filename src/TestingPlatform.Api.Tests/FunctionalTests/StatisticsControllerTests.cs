using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using TestingPlatform.Api.Tests.Extensions;
using TestingPlatform.Api.Tests.NSwag;

namespace TestingPlatform.Api.Tests.FunctionalTests
{
    public class StatisticsControllerTests
    {
        private HttpClient _httpClient;
        private UsersClient _usersClient;
        private TestsClient _testsClient;
        private QuestionsClient _questionsClient;
        private AnswersClient _answersClient;
        private StatisticsClient _sut;
        private UserDto _user;
        private Guid _testId;

        [SetUp]
        public void Setup()
        {
            _httpClient = new HttpClient();
            _usersClient = new UsersClient(_httpClient);
            _testsClient = new TestsClient(_httpClient);
            _questionsClient = new QuestionsClient(_httpClient);
            _answersClient = new AnswersClient(_httpClient);
            _sut = new StatisticsClient(_httpClient);

            _user = GetRandomUser();
            _usersClient.RegisterUserAsync(_user).GetAwaiter().GetResult();
            _testId = _testsClient.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();
        }

        [Test]
        public void GetResultList_Results_Success()
        {
            var questions = GenerateQuestion(10);

            const int resultsCount = 10;
            GenerateAnswers(questions, resultsCount, 0);

            var results = _sut.GetResultListAsync(_testId).GetAwaiter().GetResult().Deserialize<List<(Guid Id, double score)>>();
            Assert.AreEqual(resultsCount, results.Count);
        }

        [Test]
        public void GetAverageScore_AverageScore_Success()
        {
            var questions = GenerateQuestion(10);

            const int resultsCount = 10;
            GenerateAnswers(questions, resultsCount, 3);

            var average = _sut.GetAverageScoreAsync(_testId).GetAwaiter().GetResult().Deserialize<double>();
            Assert.AreEqual(0.7d, average, 0.00001d);
        }

        [Test]
        public void GetScorePerQuestions_ScorePerQuestions_Success()
        {
            var questions = GenerateQuestion(4);

            for (var i = 0; i < questions.Count; i++) 
                GenerateAnswers(questions, 1, i);

            var questionAndScores = _sut.GetScorePerQuestionsAsync(_testId).GetAwaiter().GetResult()
                .Deserialize<List<QuestionAndScore>>();

            for (var i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                var score = questionAndScores.First(s => s.Question == question.Question).Score;
                Assert.AreEqual((i + 1) * (1d / questions.Count), score, 0.00001d);
            }
        }

        [Test]
        public void GetScoreDistribution_ScoreDistribution_Success()
        {
            var questions = GenerateQuestion(5);

            for (var i = 1; i <= questions.Count; i++) 
                GenerateAnswers(questions, 1, i);

            var scoreDistribution = _sut.GetScoreDistributionAsync(_testId).GetAwaiter().GetResult()
                .Deserialize<List<double>>();
            
            Assert.AreEqual(new List<double> { 0.4, 0.2, 0.2, 0.2 }, scoreDistribution);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
        }

        private List<QuestionDto> GenerateQuestion(int questionsCount)
        {
            for (var i = 0; i < questionsCount; i++)
                _questionsClient.AddAsync(_testId).GetAwaiter().GetResult();

            var questions = _questionsClient.GetListAsync(_testId).GetAwaiter().GetResult().Deserialize<List<QuestionDto>>();
            foreach (var question in questions)
            {
                question.Question = GetRandomString();
                question.Answer = GetRandomString();
                _questionsClient.UpdateAsync(question).GetAwaiter().GetResult();
            }

            return questions;
        }

        private void GenerateAnswers(List<QuestionDto> questions, int resultsCount, int wrongAnswersCount)
        {
            var answers = questions.Select(q => 
                new AnswerDto { QuestionId = q.Id, UserAnswer = q.Answer }).ToList();

            for (var i = 0; i < wrongAnswersCount; i++) 
                answers[i].UserAnswer = GetRandomString();
            
            for (var i = 0; i < resultsCount; i++) 
                _answersClient.SendListAsync(answers).GetAwaiter().GetResult();
        }

        private string GetRandomString() => Guid.NewGuid().ToString();
        
        private UserDto GetRandomUser() => new() { Login = GetRandomString(), Password = GetRandomString() };

        private class QuestionAndScore
        {
            public string Question { get; set; }
            public double Score { get; set; }
        }
    }
}