using Microsoft.AspNetCore.Mvc;

namespace ServiceLayerApi.Controllers.Status
{
    [ApiController]
    [Route("status")]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        [Route("ping")]
        public ActionResult<string> Ping()
        {
            return Ok("Pong");
        }
    }
}