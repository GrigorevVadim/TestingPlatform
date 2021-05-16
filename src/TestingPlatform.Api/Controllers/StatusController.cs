using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TestingPlatform.Api.Controllers
{
    [Authorize]
    [Route("api/v1/status")]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public ActionResult GetStatus()
        {
            return Ok();
        }
    }
}