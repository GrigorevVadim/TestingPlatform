using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using TestingPlatform.Api.Models.Dal;
using TestingPlatform.Api.Models.Dto;

namespace TestingPlatform.Api.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserDto, UserDbo>();
            });
            var mapper = new Mapper(config);
            services.AddSingleton<IMapper>(mapper);

            return services;
        }
    }
}