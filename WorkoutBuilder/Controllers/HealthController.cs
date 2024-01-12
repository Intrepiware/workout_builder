using Microsoft.AspNetCore.Mvc;

namespace WorkoutBuilder.Controllers
{
    public class HealthController : Controller
    {
        public IActionResult Ping()
        {
            return NoContent();
        }
    }
}
