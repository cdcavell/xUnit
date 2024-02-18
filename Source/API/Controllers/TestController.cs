using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class TestController : ControllerBase
    {
        [Authorize]
        [HttpGet("Test/Hello")]
        [ApiExplorerSettings(IgnoreApi = false)]
        public ActionResult Hello()
        {
            return Ok("Hello");
        }
    }
}
