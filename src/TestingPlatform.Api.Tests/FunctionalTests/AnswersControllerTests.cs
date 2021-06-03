using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using TestingPlatform.Api.Tests.Extensions;
using TestingPlatform.Api.Tests.NSwag;

namespace TestingPlatform.Api.Tests.FunctionalTests
{
    public class AnswersControllerTests
    {
        private HttpClient _httpClient;
        private UsersClient _usersClient;
        private TestsClient _testsClient;
        private QuestionsClient _questionsClient;
        private AnswersClient _sut;
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
            _sut = new AnswersClient(_httpClient);
            
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
        public void SendList_Answers_Success()
        {
            var resultId = _sut.SendListAsync(GetAnswersForSending()).GetAwaiter().GetResult().Deserialize<Guid>();
            var response = _sut.GetListAsync(resultId).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.StatusCode);
        }

        [Test]
        public void SendList_AnswersWithNonexistentElement_Fail()
        {
            var answersWithNonexistentElement = GetAnswersForSending();
            answersWithNonexistentElement.Add(new AnswerDto { QuestionId = Guid.NewGuid(), UserAnswer = GetRandomString() });

            var exception = Assert.Throws<ApiException>(() =>
                _sut.SendListAsync(answersWithNonexistentElement).GetAwaiter().GetResult());
            Assert.AreEqual(400, exception.StatusCode);
        }

        [Test]
        public void SendList_AnswersWithDuplicatedElements_Fail()
        {
            var answersWithDuplicatedElements = GetAnswersForSending();
            answersWithDuplicatedElements.Add(answersWithDuplicatedElements[0]);
            
            var exception = Assert.Throws<ApiException>(() =>
                _sut.SendListAsync(answersWithDuplicatedElements).GetAwaiter().GetResult());
            Assert.AreEqual(400, exception.StatusCode);
        }

        [Test]
        public void GetList_Answers_Success()
        {
            var answersBeforeSend = GetAnswersForSending();
            var resultId = _sut.SendListAsync(answersBeforeSend).GetAwaiter().GetResult().Deserialize<Guid>();
            var answersByResult = _sut.GetListAsync(resultId).GetAwaiter().GetResult().Deserialize<List<AnswerDto>>();

            foreach (var answerByResult in answersByResult)
            {
                var answerBeforeSend = answersBeforeSend.Single(a => a.QuestionId == answerByResult.QuestionId);
                Assert.IsTrue(answerBeforeSend.UserAnswer == answerByResult.UserAnswer);
            }
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