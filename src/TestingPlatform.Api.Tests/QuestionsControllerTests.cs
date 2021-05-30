using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using TestingPlatform.Api.Tests.Extensions;
using TestingPlatform.Api.Tests.NSwag;

namespace TestingPlatform.Api.Tests
{
    public class QuestionsControllerTests
    {
        HttpClient httpClient;
        UsersClient usersClient;
        TestsClient testsClient;
        private QuestionsClient sut;
        UserDto user;
        Guid testId;

        [SetUp]
        public void Setup()
        {
            httpClient = new HttpClient();
            usersClient = new UsersClient(httpClient);
            testsClient = new TestsClient(httpClient);
            sut = new QuestionsClient(httpClient);
            
            user = GetRandomUser();
            usersClient.RegisterUserAsync(user).GetAwaiter().GetResult();
            testId = testsClient.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();
        }

        [Test]
        public void GetList_NewTestEmptyList_Success()
        {
            var newTestId = testsClient.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();

            var response = sut.GetListAsync(newTestId).GetAwaiter().GetResult();
            Assert.AreEqual(response.StatusCode, 200);
            Assert.IsEmpty(response.Deserialize<List<QuestionDto>>());
        }

        [Test]
        public void Add_EmptyQuestion_Success()
        {
            var questionId = sut.AddAsync(testId).GetAwaiter().GetResult().Deserialize<Guid>();

            var list = sut.GetListAsync(testId).GetAwaiter().GetResult().Deserialize<List<QuestionDto>>();
            var question = list.FirstOrDefault(t => t.Id == questionId);
            
            Assert.IsNotNull(question);
        }

        [Test]
        public void Add_QuestionToAnotherUserTest_Fail()
        {
            var newUser = GetRandomUser();
            usersClient.RegisterUserAsync(newUser).GetAwaiter().GetResult();
            
            Assert.Throws<ApiException>(() =>
                sut.AddAsync(testId).GetAwaiter().GetResult());
        }

        [Test]
        public void Add_NonexistentTest_Fail()
        {
            Assert.Throws<ApiException>(() =>
                sut.AddAsync(Guid.NewGuid()).GetAwaiter().GetResult());
        }

        [Test]
        public void Update_AlreadyExistsQuestion_Success()
        {
            var newText = "new text";
            
            var questionId = sut.AddAsync(testId).GetAwaiter().GetResult().Deserialize<Guid>();
            var question = sut.GetListAsync(testId).GetAwaiter().GetResult().Deserialize<List<QuestionDto>>()
                .Single(q => q.Id == questionId);
            
            Assert.AreEqual(question.Question, null);
            Assert.AreEqual(question.Answer, null);
            question.Question = newText;
            question.Answer = newText;

            var response = sut.UpdateAsync(question).GetAwaiter().GetResult();
            Assert.AreEqual(response.StatusCode, 200);
            
            var updatedQuestion = sut.GetListAsync(testId).GetAwaiter().GetResult().Deserialize<List<QuestionDto>>()
                .Single(q => q.Id == questionId);
            Assert.AreEqual(updatedQuestion.Question, newText);
            Assert.AreEqual(updatedQuestion.Answer, newText);
        }

        [Test]
        public void Update_NonexistentQuestion_Fail()
        {
            var newText = "new text";
            var test = new QuestionDto { Id = Guid.NewGuid(), Question = newText, Answer = newText };

            Assert.Throws<ApiException>(() =>
                sut.UpdateAsync(test).GetAwaiter().GetResult());
        }

        [Test]
        public void Update_QuestionByAnotherUser_Fail()
        {
            var newText = "new text";
            
            var questionId = sut.AddAsync(testId).GetAwaiter().GetResult().Deserialize<Guid>();
            var question = sut.GetListAsync(testId).GetAwaiter().GetResult().Deserialize<List<QuestionDto>>()
                .Single(q => q.Id == questionId);
            
            Assert.AreEqual(question.Question, null);
            Assert.AreEqual(question.Answer, null);
            question.Question = newText;
            question.Answer = newText;

            usersClient.RegisterUserAsync(GetRandomUser()).GetAwaiter().GetResult();
            
            Assert.Throws<ApiException>(() =>
                sut.UpdateAsync(question).GetAwaiter().GetResult());
            
            var notUpdatedQuestion = sut.GetListAsync(testId).GetAwaiter().GetResult().Deserialize<List<QuestionDto>>()
                .Single(q => q.Id == questionId);
            Assert.AreEqual(notUpdatedQuestion.Question, null);
            Assert.AreEqual(notUpdatedQuestion.Answer, null);
        }

        [Test]
        public void Remove_AlreadyExistsQuestion_Success()
        {
            var questionId = sut.AddAsync(testId).GetAwaiter().GetResult().Deserialize<Guid>();
            
            var response = sut.RemoveAsync(questionId).GetAwaiter().GetResult();
            Assert.AreEqual(response.StatusCode, 200);
            
            var question = sut.GetListAsync(testId).GetAwaiter().GetResult().Deserialize<List<QuestionDto>>()
                .FirstOrDefault(q => q.Id == questionId);
            Assert.IsNull(question);
        }

        [Test]
        public void Remove_NonexistentQuestion_Fail()
        {
            Assert.Throws<ApiException>(() =>
                sut.RemoveAsync(Guid.NewGuid()).GetAwaiter().GetResult());
        }
        
        [TearDown]
        public void TearDown()
        {
            httpClient.Dispose();
        }

        private string GetRandomString() => Guid.NewGuid().ToString();
        
        private UserDto GetRandomUser() => new() { Login = GetRandomString(), Password = GetRandomString() };
    }
}