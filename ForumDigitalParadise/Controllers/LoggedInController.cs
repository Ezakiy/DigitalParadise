using Microsoft.AspNetCore.Mvc;

namespace ForumDigitalParadise.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoggedInController : ControllerBase
    {
        [HttpGet("IsLoggedIn")]
        public IActionResult IsLoggedIn()
        {
    
            bool isLoggedIn = HttpContext.User.Identity.IsAuthenticated;

            return Ok(new { isLoggedIn });
        }
    }
}
