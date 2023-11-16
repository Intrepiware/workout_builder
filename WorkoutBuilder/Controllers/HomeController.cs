using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using WorkoutBuilder.Data;
using WorkoutBuilder.Models;
using WorkoutBuilder.Services;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Controllers
{
    public class HomeController : Controller
    {
        public IRepository<Exercise> ExerciseRepository { protected get; init; }
        public IRepository<Timing> TimingRepository { protected get; init; }
        public IWorkoutService WorkoutService { protected get; init; }
        public IEmailService EmailService { protected get; init; }
        public IConfiguration Configuration { protected get; init; }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 3600)]
        public IActionResult Timings()
        {
            var timings = TimingRepository.GetAll().OrderBy(x => x.Name).ToList();
            return Json(timings);
        }

        public IActionResult Workout(string? timing, string? focus)
        {
            Services.Models.Focus? requestedFocus = null;
            if (Enum.TryParse<Services.Models.Focus>(focus, true, out var parsedFocus))
                requestedFocus = parsedFocus;

            var result = WorkoutService.Generate(new WorkoutGenerationRequestModel { Focus = requestedFocus, Timing = timing });

            return Json(result);
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(HomeContactRequestModel data)
        {
            var toEmail = Configuration["ContactFormToAddress"];
            var body = @$"Name: {data.Name}
Location: {data.Location}
Email: {data.Email}
Subject: {data.Subject}
Message: {data.Message}";
            EmailService.Send(toEmail, "Contact Form Submission", body);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}