using Microsoft.AspNetCore.Mvc;

namespace WorkoutBuilder.Controllers
{
    public class TimingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
