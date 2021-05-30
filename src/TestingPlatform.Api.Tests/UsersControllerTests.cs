using System;
using System.Net.Http;
using NUnit.Framework;
using TestingPlatform.Api.Tests.NSwag;

namespace TestingPlatform.Api.Tests
{
    public class UsersControllerTests
    {
        HttpClient httpClient;
        UsersClient sut;
        TestsClient testsClient;
        UserDto user;

        [SetUp]
        public void Setup()
        {
            httpClient = new HttpClient();
            sut = new UsersClient(httpClient);
            testsClient = new TestsClient(httpClient);
            
            user = GetRandomUser();
            sut.RegisterUserAsync(user).GetAwaiter().GetResult();
            sut.LogoutAsync().GetAwaiter().GetResult();
        }

        [Test]
        public void RegisterUser_NewUser_Success()
        {
            AssertFailStatusCode();

            sut.RegisterUserAsync(GetRandomUser()).GetAwaiter().GetResult();
            AssertSussesStatusCode();
        }

        [Test]
        public void RegisterUser_AlreadyExistsUser_Fail()
        {
            AssertFailStatusCode();

            Assert.Throws<ApiException>(() =>
                sut.RegisterUserAsync(user).GetAwaiter().GetResult());
        }

        [Test]
        public void Login_AlreadyExistsUser_Success()
        {
            AssertFailStatusCode();

            sut.LoginAsync(user).GetAwaiter().GetResult();
            AssertSussesStatusCode();
        }

        [Test]
        public void Login_NonexistentUser_Fail()
        {
            AssertFailStatusCode();

            Assert.Throws<ApiException>(() => 
                sut.LoginAsync(GetRandomUser()).GetAwaiter().GetResult());
            AssertFailStatusCode();
        }

        [Test]
        public void Logout_AlreadyLoggedIn_Success()
        {
            AssertFailStatusCode();

            sut.LoginAsync(user).GetAwaiter().GetResult();
            AssertSussesStatusCode();
            
            sut.LogoutAsync().GetAwaiter().GetResult();
            AssertFailStatusCode();
        }

        [Test]
        public void Logout_NotLoggedIn_Success()
        {
            AssertFailStatusCode();
            
            sut.LogoutAsync().GetAwaiter().GetResult();
            AssertFailStatusCode();
        }

        [TearDown]
        public void TearDown()
        {
            httpClient.Dispose();
        }

        private void AssertSussesStatusCode()
        {
            var response = testsClient.GetListAsync().GetAwaiter().GetResult();
            Assert.AreEqual(response.StatusCode, 200);
        }

        private void AssertFailStatusCode() => 
            Assert.Throws<ApiException>(() => testsClient.GetListAsync().GetAwaiter().GetResult());

        private string GetRandomString() => Guid.NewGuid().ToString();
        
        private UserDto GetRandomUser() => new() { Login = GetRandomString(), Password = GetRandomString() };
    }
}