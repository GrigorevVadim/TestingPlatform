using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Api.Models;
using TestingPlatform.Api.Models.Dal;

namespace TestingPlatform.Api.Core
{
    public abstract class CustomControllerBase : ControllerBase
    {
        protected readonly IMapper Mapper;
        protected readonly ModelsDataContext ModelsContext;

        protected CustomControllerBase(IMapper mapper, ModelsDataContext modelsContext)
        {
            Mapper = mapper;
            ModelsContext = modelsContext;
        }

        protected async Task<UserDbo> GetUser() => 
            await ModelsContext.Users.SingleAsync(u => u.Id == GetUserId());

        protected int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}