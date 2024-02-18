using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Controller]
    public class HomeController : Controller
    {
        [HttpGet("/")]
        [HttpGet("Home/")]
        [HttpGet("Home/Index")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}
