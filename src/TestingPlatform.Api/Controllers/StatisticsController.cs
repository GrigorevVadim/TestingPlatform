using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Api.Models;
using TestingPlatform.Api.Models.Dto;

namespace TestingPlatform.Api.Controllers
{
    [Authorize]
    [Route("api/v1/Statistics")]
    public class StatisticsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ModelsDataContext _context;

        public StatisticsController(IMapper mapper, ModelsDataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("GetResultList")]
        public async Task<ActionResult> GetListAsync(Guid testId)
        {
            var results = await _context.Results
                .Include(r => r.User)
                .Where(r => r.Test.Id == testId)
                .ToListAsync();
            
            return Ok(_mapper.Map<List<ResultDto>>(results));
        }
    }
}