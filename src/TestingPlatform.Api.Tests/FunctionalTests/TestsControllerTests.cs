using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using TestingPlatform.Api.Tests.Extensions;
using TestingPlatform.Api.Tests.NSwag;

namespace TestingPlatform.Api.Tests.FunctionalTests
{
    public class TestsControllerTests
    {
        private HttpClient _httpClient;
        private UsersClient _usersClient;
        private TestsClient _sut;
        private UserDto _user;

        [SetUp]
        public void Setup()
        {
            _httpClient = new HttpClient();
            _usersClient = new UsersClient(_httpClient);
            _sut = new TestsClient(_httpClient);
            
            _user = GetRandomUser();
            _usersClient.RegisterUserAsync(_user).GetAwaiter().GetResult();
        }

        [Test]
        public void GetList_NewUserEmptyList_Success()
        {
            _usersClient.RegisterUserAsync(GetRandomUser()).GetAwaiter().GetResult();

            var response = _sut.GetListAsync().GetAwaiter().GetResult();
            Assert.AreEqual(200, response.StatusCode);
            Assert.IsEmpty(response.Deserialize<List<TestDto>>());
        }

        [Test]
        public void Add_EmptyTest_Success()
        {
            var testId = _sut.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();

            var list = _sut.GetListAsync().GetAwaiter().GetResult().Deserialize<List<TestDto>>();
            var test = list.FirstOrDefault(t => t.Id == testId);
            
            Assert.IsNotNull(test);
        }

        [Test]
        public void Update_AlreadyExistsTest_Success()
        {
            var testName = "new test name";
            
            var testId = _sut.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();
            var test = _sut.GetAsync(testId).GetAwaiter().GetResult().Deserialize<TestDto>();
            
            Assert.AreEqual(null, test.Name);
            test.Name = testName;

            var response = _sut.UpdateAsync(test).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.StatusCode);
            
            var updatedTest = _sut.GetAsync(testId).GetAwaiter().GetResult().Deserialize<TestDto>();
            Assert.AreEqual(testName, updatedTest.Name);
        }

        [Test]
        public void Update_NonexistentTest_Fail()
        {
            var test = new TestDto { Id = Guid.NewGuid(), Name = "new name" };

            var exception = Assert.Throws<ApiException>(() =>
                _sut.UpdateAsync(test).GetAwaiter().GetResult());
            Assert.AreEqual(400, exception.StatusCode);
        }

        [Test]
        public void Update_TestByAnotherUser_Fail()
        {
            var testName = "new test name";
            
            var testId = _sut.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();
            var test = _sut.GetAsync(testId).GetAwaiter().GetResult().Deserialize<TestDto>();
            
            Assert.AreEqual(null, test.Name);
            test.Name = testName;

            _usersClient.RegisterUserAsync(GetRandomUser()).GetAwaiter().GetResult();
            
            var exception = Assert.Throws<ApiException>(() =>
                _sut.UpdateAsync(test).GetAwaiter().GetResult());
            Assert.AreEqual(403, exception.StatusCode);
            
            var notUpdatedTest = _sut.GetAsync(testId).GetAwaiter().GetResult().Deserialize<TestDto>();
            Assert.AreEqual(null, notUpdatedTest.Name);
        }

        [Test]
        public void Get_AlreadyExistsTest_Success()
        {
            var testId = _sut.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();
            var response = _sut.GetAsync(testId).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.StatusCode);
        }

        [Test]
        public void Get_NonexistentTest_Fail()
        {
            var exception = Assert.Throws<ApiException>(() =>
                _sut.GetAsync(Guid.NewGuid()).GetAwaiter().GetResult());
            Assert.AreEqual(400, exception.StatusCode);
        }

        [Test]
        public void Get_TestByAnotherUser_Success()
        {
            var testId = _sut.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();

            _usersClient.RegisterUserAsync(GetRandomUser()).GetAwaiter().GetResult();
            var response = _sut.GetAsync(testId).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.StatusCode);
        }

        [Test]
        public void Remove_AlreadyExistsTest_Success()
        {
            var testId = _sut.AddAsync().GetAwaiter().GetResult().Deserialize<Guid>();
            
            var response = _sut.RemoveAsync(testId).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.StatusCode);
            
            var exception = Assert.Throws<ApiException>(() =>
                _sut.GetAsync(testId).GetAwaiter().GetResult());
            Assert.AreEqual(400, exception.StatusCode);
        }

        [Test]
        public void Remove_NonexistentTest_Fail()
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