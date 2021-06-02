using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Api.Core;
using TestingPlatform.Api.Models;
using TestingPlatform.Api.Models.Dal;
using TestingPlatform.Api.Models.Dto;

namespace TestingPlatform.Api.Controllers
{
    [Authorize]
    [Route("api/v1/Answers")]
    public class AnswersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ModelsDataContext _context;
        private readonly AnswersHandler _answersHandler;

        public AnswersController(IMapper mapper, ModelsDataContext context, AnswersHandler answersHandler)
        {
            _mapper = mapper;
            _context = context;
            _answersHandler = answersHandler;
        }

        [HttpPost("SendList")]
        public async Task<ActionResult> SendListAsync([FromBody] List<AnswerDto> answersDto)
        {
            var answersDbo = _mapper.Map<List<AnswerDbo>>(answersDto);

            var test = await _answersHandler.GetTestAsync(answersDbo);
            if (test == null)
                return BadRequest("Answers contains wrong elements");
            
            return Ok(await _answersHandler.SaveAnswersAsync(answersDbo, test, await GetUser()));
        }

        [HttpGet("List")]
        public async Task<ActionResult> GetListAsync(Guid resultId)
        {
            var result = await _context.Results.Include(r => r.Answers).FirstOrDefaultAsync(r => r.Id == resultId);
            if (result == null)
                return BadRequest("Result does not exist");

            var answers = _mapper.Map<List<AnswerDto>>(result.Answers);
            
            return Ok(answers);
        }
        
        private async Task<UserDbo> GetUser() => 
            await _context.Users.SingleAsync(u => u.Id == GetUserId());

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}