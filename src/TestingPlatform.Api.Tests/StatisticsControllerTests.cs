using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using TestingPlatform.Api.Tests.Extensions;
using TestingPlatform.Api.Tests.NSwag;

namespace TestingPlatform.Api.Tests
{
    public class StatisticsControllerTests
    {
        HttpClient httpClient;
        UsersClient usersClient;
        TestsClient testsClient;
        QuestionsClient questionsClient;
        AnswersClient answersClient;
        StatisticsClient sut;
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
            answersClient = new AnswersClient(httpClient);
            sut = new StatisticsClient(httpClient);

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
        public void GetList_Results_Success()
        {
            const int count = 10;
            for (var i = 0; i < count; i++) 
                answersClient.SendListAsync(GetAnswersForSending()).GetAwaiter().GetResult();

            var results = sut.GetListAsync(testId).GetAwaiter().GetResult().Deserialize<List<(Guid Id, double score)>>();
            Assert.AreEqual(results.Count, count);
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