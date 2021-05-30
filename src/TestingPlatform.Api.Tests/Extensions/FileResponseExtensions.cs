using System.IO;
using Newtonsoft.Json;
using TestingPlatform.Api.Tests.NSwag;

namespace TestingPlatform.Api.Tests.Extensions
{
    public static class FileResponseExtensions
    {
        public static T Deserialize<T>(this FileResponse fileResponse)
        {
            using var sr = new StreamReader(fileResponse.Stream);
            using var reader = new JsonTextReader(sr);
            var serializer = new JsonSerializer();
            return serializer.Deserialize<T>(reader);
        }
    }
}