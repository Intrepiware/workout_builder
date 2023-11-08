using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WorkoutBuilder.Data;
using WorkoutBuilder.Models;
using WorkoutBuilder.Services;

namespace WorkoutBuilder.Controllers
{
    public class HomeController : Controller
    {
        //public required ITestService TestService { protected get; init; }
        public required IRepository<Focus> FocusRepository { protected get; init; }
        public required IRepository<Exercise> ExerciseRepository { protected get; init; }
        public required IRepository<Timing> TimingRepository { protected get; init; }

        public IActionResult Index()
        {
            var data = TimingRepository.GetAll().ToList();
            return Json(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}