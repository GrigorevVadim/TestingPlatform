using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using TestingPlatform.Api.Tests.Extensions;
using TestingPlatform.Api.Tests.NSwag;

namespace TestingPlatform.Api.Tests
{
    public class AnswersControllerTests
    {
        HttpClient httpClient;
        UsersClient usersClient;
        TestsClient testsClient;
        QuestionsClient questionsClient;
        private AnswersClient sut;
        UserDto user;
        Guid testId;
        List<QuestionDto> questions;

        [SetUp]
        public void Setup()
        {
            httpClient = new HttpClient();
            usersClient = new UsersClient(httpClient);
            testsClient = new TestsClient(httpClient);
            questionsClient = new QuestionsClient(httpClient);
            sut = new AnswersClient(httpClient);
            
            user = GetRandomUser();
            usersClient.RegisterUserAsync(user).GetAwaiter().GetResult();
            testId = testsClient.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();

            for (var i = 0; i < 10; i++)
                questionsClient.AddAsync(testId).GetAwaiter().GetResult();

            questions = questionsClient.GetListAsync(testId).GetAwaiter().GetResult().Deserialize<List<QuestionDto>>();
            foreach (var question in questions)
            {
                question.Question = GetRandomString();
                question.Answer = GetRandomString();
                questionsClient.UpdateAsync(question).GetAwaiter().GetResult();
            }
        }

        [Test]
        public void SendList_Answers_Success()
        {
            var resultId = sut.SendListAsync(GetAnswersForSending()).GetAwaiter().GetResult().Deserialize<Guid>();
            var response = sut.GetListAsync(resultId).GetAwaiter().GetResult();
            Assert.AreEqual(response.StatusCode, 200);
        }

        [Test]
        public void SendList_AnswersWithNonexistentElement_Fail()
        {
            var answersWithNonexistentElement = GetAnswersForSending();
            answersWithNonexistentElement.Add(new AnswerDto { QuestionId = Guid.NewGuid(), UserAnswer = GetRandomString() });

            Assert.Throws<ApiException>(() =>
                sut.SendListAsync(answersWithNonexistentElement).GetAwaiter().GetResult());
        }

        [Test]
        public void SendList_AnswersWithDuplicatedElements_Fail()
        {
            var answersWithDuplicatedElements = GetAnswersForSending();
            answersWithDuplicatedElements.Add(answersWithDuplicatedElements[0]);
            
            Assert.Throws<ApiException>(() =>
                sut.SendListAsync(answersWithDuplicatedElements).GetAwaiter().GetResult());
        }

        [Test]
        public void GetList_Answers_Success()
        {
            var answersBeforeSend = GetAnswersForSending();
            var resultId = sut.SendListAsync(answersBeforeSend).GetAwaiter().GetResult().Deserialize<Guid>();
            var answersByResult = sut.GetListAsync(resultId).GetAwaiter().GetResult().Deserialize<List<AnswerDto>>();

            foreach (var answerByResult in answersByResult)
            {
                var answerBeforeSend = answersBeforeSend.Single(a => a.QuestionId == answerByResult.QuestionId);
                Assert.IsTrue(answerBeforeSend.UserAnswer == answerByResult.UserAnswer);
            }
        }

        private List<AnswerDto> GetAnswersForSending()
        {
            var answers = questions.Select(q => 
                new AnswerDto { QuestionId = q.Id, UserAnswer = q.Answer }).ToList();

            for (var i = 0; i < 4; i++) 
                answers[i * 3].UserAnswer = GetRandomString();

            return answers;
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