using Microsoft.AspNetCore.Mvc;

namespace WorkoutBuilder.Controllers
{
    public class ExercisesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
