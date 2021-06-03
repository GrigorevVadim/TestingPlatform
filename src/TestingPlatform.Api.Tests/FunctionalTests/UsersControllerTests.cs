using System;
using System.Net.Http;
using NUnit.Framework;
using TestingPlatform.Api.Tests.NSwag;

namespace TestingPlatform.Api.Tests.FunctionalTests
{
    public class UsersControllerTests
    {
        private HttpClient _httpClient;
        private UsersClient _sut;
        private TestsClient _testsClient;
        private UserDto _user;

        [SetUp]
        public void Setup()
        {
            _httpClient = new HttpClient();
            _sut = new UsersClient(_httpClient);
            _testsClient = new TestsClient(_httpClient);
            
            _user = GetRandomUser();
            _sut.RegisterUserAsync(_user).GetAwaiter().GetResult();
            _sut.LogoutAsync().GetAwaiter().GetResult();
        }

        [Test]
        public void RegisterUser_NewUser_Success()
        {
            AssertFailStatusCode();

            _sut.RegisterUserAsync(GetRandomUser()).GetAwaiter().GetResult();
            AssertSussesStatusCode();
        }

        [Test]
        public void RegisterUser_AlreadyExistsUser_Fail()
        {
            AssertFailStatusCode();

            var exception = Assert.Throws<ApiException>(() =>
                _sut.RegisterUserAsync(_user).GetAwaiter().GetResult());
            Assert.AreEqual(400, exception.StatusCode);
        }

        [Test]
        public void Login_AlreadyExistsUser_Success()
        {
            AssertFailStatusCode();

            _sut.LoginAsync(_user).GetAwaiter().GetResult();
            AssertSussesStatusCode();
        }

        [Test]
        public void Login_NonexistentUser_Fail()
        {
            AssertFailStatusCode();

            var exception = Assert.Throws<ApiException>(() => 
                _sut.LoginAsync(GetRandomUser()).GetAwaiter().GetResult());
            Assert.AreEqual(403, exception.StatusCode);
            
            AssertFailStatusCode();
        }

        [Test]
        public void Logout_AlreadyLoggedIn_Success()
        {
            AssertFailStatusCode();

            _sut.LoginAsync(_user).GetAwaiter().GetResult();
            AssertSussesStatusCode();
            
            _sut.LogoutAsync().GetAwaiter().GetResult();
            AssertFailStatusCode();
        }

        [Test]
        public void Logout_NotLoggedIn_Success()
        {
            AssertFailStatusCode();
            
            _sut.LogoutAsync().GetAwaiter().GetResult();
            AssertFailStatusCode();
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
        }

        private void AssertSussesStatusCode()
        {
            var response = _testsClient.GetListAsync().GetAwaiter().GetResult();
            Assert.AreEqual(200, response.StatusCode);
        }

        private void AssertFailStatusCode()
        {
            var exception = Assert.Throws<ApiException>(() => _testsClient.GetListAsync().GetAwaiter().GetResult());
            Assert.AreEqual(401, exception.StatusCode);
        }

        private string GetRandomString() => Guid.NewGuid().ToString();
        
        private UserDto GetRandomUser() => new() { Login = GetRandomString(), Password = GetRandomString() };
    }
}