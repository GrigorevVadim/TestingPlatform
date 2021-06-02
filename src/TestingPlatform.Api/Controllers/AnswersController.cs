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
    public class AnswersController : CustomControllerBase
    {
        private readonly AnswersHandler _answersHandler;

        public AnswersController(IMapper mapper, ModelsDataContext modelsContext, AnswersHandler answersHandler)
            : base(mapper, modelsContext)
        {
            _answersHandler = answersHandler;
        }

        [HttpPost("SendList")]
        public async Task<ActionResult> SendListAsync([FromBody] List<AnswerDto> answersDto)
        {
            var answersDbo = Mapper.Map<List<AnswerDbo>>(answersDto);

            var test = await _answersHandler.GetTestAsync(answersDbo);
            if (test == null)
                return BadRequest("Answers contains wrong elements");
            
            return Ok(await _answersHandler.SaveAnswersAsync(answersDbo, test, await GetUser()));
        }

        [HttpGet("List")]
        public async Task<ActionResult> GetListAsync(Guid resultId)
        {
            var result = await ModelsContext.Results.Include(r => r.Answers).FirstOrDefaultAsync(r => r.Id == resultId);
            if (result == null)
                return BadRequest("Result does not exist");

            var answers = Mapper.Map<List<AnswerDto>>(result.Answers);
            
            return Ok(answers);
        }
    }
}