using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WorkoutBuilder.Data;
using WorkoutBuilder.Models;
using WorkoutBuilder.Services;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Controllers
{
    public class HomeController : Controller
    {
        public required IRepository<Exercise> ExerciseRepository { protected get; init; }
        public required IRepository<Timing> TimingRepository { protected get; init; }
        public required IWorkoutService WorkoutService { protected get; init; }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Workout(string? timing, string? focus)
        {
            Services.Models.Focus? requestedFocus = null;
            if (Enum.TryParse<Services.Models.Focus>(focus, true, out var parsedFocus))
                requestedFocus = parsedFocus;

            var result = WorkoutService.Generate(new WorkoutGenerationRequestModel { Focus = requestedFocus, Timing = timing });

            return Json(result);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}