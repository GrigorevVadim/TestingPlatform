using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Api.Core;
using TestingPlatform.Api.Models;
using TestingPlatform.Api.Models.Dto;

namespace TestingPlatform.Api.Controllers
{
    [Authorize]
    [Route("api/v1/Statistics")]
    public class StatisticsController : CustomControllerBase
    {
        public StatisticsController(IMapper mapper, ModelsDataContext modelsContext)
            : base(mapper, modelsContext)
        {
        }

        [HttpGet("GetResultList")]
        public async Task<ActionResult> GetListAsync(Guid testId)
        {
            var results = await ModelsContext.Results
                .Include(r => r.User)
                .Where(r => r.Test.Id == testId)
                .ToListAsync();
            
            return Ok(Mapper.Map<List<ResultDto>>(results));
        }
    }
}