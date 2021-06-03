using System;
using System.Collections.Generic;
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
        private List<QuestionDto> _questions;

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

            for (var i = 0; i < 10; i++)
                _questionsClient.AddAsync(_testId).GetAwaiter().GetResult();

            _questions = _questionsClient.GetListAsync(_testId).GetAwaiter().GetResult().Deserialize<List<QuestionDto>>();
            foreach (var question in _questions)
            {
                question.Question = GetRandomString();
                question.Answer = GetRandomString();
                _questionsClient.UpdateAsync(question).GetAwaiter().GetResult();
            }
        }

        [Test]
        public void GetList_Results_Success()
        {
            const int count = 10;
            for (var i = 0; i < count; i++) 
                _answersClient.SendListAsync(GetAnswersForSending()).GetAwaiter().GetResult();

            var results = _sut.GetListAsync(_testId).GetAwaiter().GetResult().Deserialize<List<(Guid Id, double score)>>();
            Assert.AreEqual(count, results.Count);
        }

        private List<AnswerDto> GetAnswersForSending()
        {
            var answers = _questions.Select(q => 
                new AnswerDto { QuestionId = q.Id, UserAnswer = q.Answer }).ToList();

            for (var i = 0; i < 4; i++) 
                answers[i * 3].UserAnswer = GetRandomString();

            return answers;
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
        }

        private string GetRandomString() => Guid.NewGuid().ToString();
        
        private UserDto GetRandomUser() => new() { Login = GetRandomString(), Password = GetRandomString() };
    }
}