using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using TestingPlatform.Api.Tests.Extensions;
using TestingPlatform.Api.Tests.NSwag;

namespace TestingPlatform.Api.Tests.FunctionalTests
{
    public class QuestionsControllerTests
    {
        private HttpClient _httpClient;
        private UsersClient _usersClient;
        private TestsClient _testsClient;
        private QuestionsClient _sut;
        private UserDto _user;
        private Guid _testId;

        [SetUp]
        public void Setup()
        {
            _httpClient = new HttpClient();
            _usersClient = new UsersClient(_httpClient);
            _testsClient = new TestsClient(_httpClient);
            _sut = new QuestionsClient(_httpClient);
            
            _user = GetRandomUser();
            _usersClient.RegisterUserAsync(_user).GetAwaiter().GetResult();
            _testId = _testsClient.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();
        }

        [Test]
        public void GetList_NewTestEmptyList_Success()
        {
            var newTestId = _testsClient.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();

            var response = _sut.GetListAsync(newTestId).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.StatusCode);
            Assert.IsEmpty(response.Deserialize<List<QuestionDto>>());
        }

        [Test]
        public void Add_EmptyQuestion_Success()
        {
            var questionId = _sut.AddAsync(_testId).GetAwaiter().GetResult().Deserialize<Guid>();

            var list = _sut.GetListAsync(_testId).GetAwaiter().GetResult().Deserialize<List<QuestionDto>>();
            var question = list.FirstOrDefault(t => t.Id == questionId);
            
            Assert.IsNotNull(question);
        }

        [Test]
        public void Add_QuestionToAnotherUserTest_Fail()
        {
            var newUser = GetRandomUser();
            _usersClient.RegisterUserAsync(newUser).GetAwaiter().GetResult();
            
            var exception = Assert.Throws<ApiException>(() =>
                _sut.AddAsync(_testId).GetAwaiter().GetResult());
            Assert.AreEqual(403, exception.StatusCode);
        }

        [Test]
        public void Add_NonexistentTest_Fail()
        {
            var exception = Assert.Throws<ApiException>(() =>
                _sut.AddAsync(Guid.NewGuid()).GetAwaiter().GetResult());
            Assert.AreEqual(400, exception.StatusCode);
        }

        [Test]
        public void Update_AlreadyExistsQuestion_Success()
        {
            var newText = "new text";
            
            var questionId = _sut.AddAsync(_testId).GetAwaiter().GetResult().Deserialize<Guid>();
            var question = _sut.GetListAsync(_testId).GetAwaiter().GetResult().Deserialize<List<QuestionDto>>()
                .Single(q => q.Id == questionId);
            
            Assert.AreEqual(null, question.Question);
            Assert.AreEqual(null, question.Answer);
            question.Question = newText;
            question.Answer = newText;

            var response = _sut.UpdateAsync(question).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.StatusCode);
            
            var updatedQuestion = _sut.GetListAsync(_testId).GetAwaiter().GetResult().Deserialize<List<QuestionDto>>()
                .Single(q => q.Id == questionId);
            Assert.AreEqual(newText, updatedQuestion.Question);
            Assert.AreEqual(newText, updatedQuestion.Answer);
        }

        [Test]
        public void Update_NonexistentQuestion_Fail()
        {
            var newText = "new text";
            var test = new QuestionDto { Id = Guid.NewGuid(), Question = newText, Answer = newText };

            var exception = Assert.Throws<ApiException>(() =>
                _sut.UpdateAsync(test).GetAwaiter().GetResult());
            Assert.AreEqual(400, exception.StatusCode);
        }

        [Test]
        public void Update_QuestionByAnotherUser_Fail()
        {
            var newText = "new text";
            
            var questionId = _sut.AddAsync(_testId).GetAwaiter().GetResult().Deserialize<Guid>();
            var question = _sut.GetListAsync(_testId).GetAwaiter().GetResult().Deserialize<List<QuestionDto>>()
                .Single(q => q.Id == questionId);
            
            Assert.AreEqual(null, question.Question);
            Assert.AreEqual(null, question.Answer);
            question.Question = newText;
            question.Answer = newText;

            _usersClient.RegisterUserAsync(GetRandomUser()).GetAwaiter().GetResult();
            
            var exception = Assert.Throws<ApiException>(() =>
                _sut.UpdateAsync(question).GetAwaiter().GetResult());
            Assert.AreEqual(403, exception.StatusCode);
            
            var notUpdatedQuestion = _sut.GetListAsync(_testId).GetAwaiter().GetResult().Deserialize<List<QuestionDto>>()
                .Single(q => q.Id == questionId);
            Assert.AreEqual(null, notUpdatedQuestion.Question);
            Assert.AreEqual(null, notUpdatedQuestion.Answer);
        }

        [Test]
        public void Remove_AlreadyExistsQuestion_Success()
        {
            var questionId = _sut.AddAsync(_testId).GetAwaiter().GetResult().Deserialize<Guid>();
            
            var response = _sut.RemoveAsync(questionId).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.StatusCode);
            
            var question = _sut.GetListAsync(_testId).GetAwaiter().GetResult().Deserialize<List<QuestionDto>>()
                .FirstOrDefault(q => q.Id == questionId);
            Assert.IsNull(question);
        }

        [Test]
        public void Remove_NonexistentQuestion_Fail()
        {
            var exception = Assert.Throws<ApiException>(() =>
                _sut.RemoveAsync(Guid.NewGuid()).GetAwaiter().GetResult());
            Assert.AreEqual(400, exception.StatusCode);
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