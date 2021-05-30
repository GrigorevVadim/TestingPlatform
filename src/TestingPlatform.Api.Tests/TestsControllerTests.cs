using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using TestingPlatform.Api.Tests.Extensions;
using TestingPlatform.Api.Tests.NSwag;

namespace TestingPlatform.Api.Tests
{
    public class TestsControllerTests
    {
        HttpClient httpClient;
        UsersClient usersClient;
        TestsClient sut;
        UserDto user;

        [SetUp]
        public void Setup()
        {
            httpClient = new HttpClient();
            usersClient = new UsersClient(httpClient);
            sut = new TestsClient(httpClient);
            
            user = GetRandomUser();
            usersClient.RegisterUserAsync(user).GetAwaiter().GetResult();
        }

        [Test]
        public void GetList_NewUserEmptyList_Success()
        {
            usersClient.RegisterUserAsync(GetRandomUser()).GetAwaiter().GetResult();

            var response = sut.GetListAsync().GetAwaiter().GetResult();
            Assert.AreEqual(response.StatusCode, 200);
            Assert.IsEmpty(response.Deserialize<List<TestDto>>());
        }

        [Test]
        public void Add_EmptyTest_Success()
        {
            var testId = sut.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();

            var list = sut.GetListAsync().GetAwaiter().GetResult().Deserialize<List<TestDto>>();
            var test = list.FirstOrDefault(t => t.Id == testId);
            
            Assert.IsNotNull(test);
        }

        [Test]
        public void Update_AlreadyExistsTest_Success()
        {
            var testName = "new test name";
            
            var testId = sut.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();
            var test = sut.GetAsync(testId).GetAwaiter().GetResult().Deserialize<TestDto>();
            
            Assert.AreEqual(test.Name, null);
            test.Name = testName;

            var response = sut.UpdateAsync(test).GetAwaiter().GetResult();
            Assert.AreEqual(response.StatusCode, 200);
            
            var updatedTest = sut.GetAsync(testId).GetAwaiter().GetResult().Deserialize<TestDto>();
            Assert.AreEqual(updatedTest.Name, testName);
        }

        [Test]
        public void Update_NonexistentTest_Fail()
        {
            var test = new TestDto { Id = Guid.NewGuid(), Name = "new name" };

            Assert.Throws<ApiException>(() =>
                sut.UpdateAsync(test).GetAwaiter().GetResult());
        }

        [Test]
        public void Update_TestByAnotherUser_Fail()
        {
            var testName = "new test name";
            
            var testId = sut.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();
            var test = sut.GetAsync(testId).GetAwaiter().GetResult().Deserialize<TestDto>();
            
            Assert.AreEqual(test.Name, null);
            test.Name = testName;

            usersClient.RegisterUserAsync(GetRandomUser()).GetAwaiter().GetResult();
            
            Assert.Throws<ApiException>(() =>
                sut.UpdateAsync(test).GetAwaiter().GetResult());
            
            var notUpdatedTest = sut.GetAsync(testId).GetAwaiter().GetResult().Deserialize<TestDto>();
            Assert.AreEqual(notUpdatedTest.Name, null);
        }

        [Test]
        public void Get_AlreadyExistsTest_Success()
        {
            var testId = sut.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();
            var response = sut.GetAsync(testId).GetAwaiter().GetResult();
            Assert.AreEqual(response.StatusCode, 200);
        }

        [Test]
        public void Get_NonexistentTest_Fail()
        {
            Assert.Throws<ApiException>(() =>
                sut.GetAsync(Guid.NewGuid()).GetAwaiter().GetResult());
        }

        [Test]
        public void Get_TestByAnotherUser_Success()
        {
            var testId = sut.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();

            usersClient.RegisterUserAsync(GetRandomUser()).GetAwaiter().GetResult();
            var response = sut.GetAsync(testId).GetAwaiter().GetResult();
            Assert.AreEqual(response.StatusCode, 200);
        }

        [Test]
        public void Remove_AlreadyExistsTest_Success()
        {
            var testId = sut.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();
            
            var response = sut.RemoveAsync(testId).GetAwaiter().GetResult();
            Assert.AreEqual(response.StatusCode, 200);
            
            Assert.Throws<ApiException>(() =>
                sut.GetAsync(testId).GetAwaiter().GetResult());
        }

        [Test]
        public void Remove_NonexistentTest_Fail()
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