using BotDetect.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using WorkoutBuilder.Data;
using WorkoutBuilder.Models;
using WorkoutBuilder.Services;
using WorkoutBuilder.Services.Impl;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Controllers
{
    public class HomeController : Controller
    {
        public IRepository<Exercise> ExerciseRepository { protected get; init; } = null!;
        public IRepository<Timing> TimingRepository { protected get; init; } = null!;
        public IRepository<Workout> WorkoutRepository { protected get; init; } = null!;
        public IWorkoutGeneratorFactory WorkoutGeneratorFactory { protected get; init; } = null!;
        public IEmailService EmailService { protected get; init; } = null!;
        public IConfiguration Configuration { protected get; init; } = null!;
        public IHomeWorkoutModelMapper HomeWorkoutModelMapper { protected get; init; } = null!;
        public IWorkoutService WorkoutService { protected get; init; } = null!;
        public IUrlBuilder UrlBuilder { protected get; init; } = null!;

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

        [ResponseCache(Duration = 3600)]
        public IActionResult Equipment()
        {
            var equipment = ExerciseRepository.GetAll().Select(x => x.Equipment).Distinct().ToList();
            return Json(equipment);
        }

        public async Task<IActionResult> Workout(string? id, string? timing, string? focus, string equipment)
        {
            if (id != null)
            {
                var savedWorkout = WorkoutRepository.GetAll().SingleOrDefault(x => x.PublicId == id);
                if (savedWorkout == null)
                    return NotFound();
                var output = HomeWorkoutModelMapper.Map(savedWorkout, id);
                return Json(output);
            }

            Services.Models.Focus? requestedFocus = null;
            if (Enum.TryParse<Services.Models.Focus>(focus, true, out var parsedFocus))
                requestedFocus = parsedFocus;

            var workoutTiming = WorkoutGeneratorFactory.GetTiming(timing);
            var result = WorkoutGeneratorFactory.GetGenerator(workoutTiming)
                            .Generate(new WorkoutGenerationRequestModel { Timing = workoutTiming, Focus = requestedFocus, Equipment = equipment?.Split('|').ToList() });
            var model = await HomeWorkoutModelMapper.Map(result);
            return Json(model);
        }

        public IActionResult TimingCalc()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [CaptchaValidationActionFilter("CaptchaCode", "ContactFormCaptcha", "Incorrect Captcha, please try again.")]
        public IActionResult Contact(HomeContactRequestModel data)
        {
            if (!ModelState.IsValid)
                return View(data);

            var toEmail = Configuration["ContactFormToAddress"];
            var body = @$"Name: {data.Name}
Location: {data.Location}
Email: {data.Email}
Subject: {data.Subject}
Message: {data.Message}";
            EmailService.Send(toEmail, "👉 Contact Form Submission", body);
            ViewBag.Success = "Thank you for your message, it has been sent successfully.";
            ModelState.Clear();
            MvcCaptcha.ResetCaptcha("ContactFormCaptcha");
            return View(new HomeContactRequestModel());
        }

        [HttpPost]
        public async Task<IActionResult> Favorite(string id)
        {
            var newId = await WorkoutService.ToggleFavorite(id);
            if (newId != id)
                Response.Headers.Add("Location", UrlBuilder.Action("Index", "Home", new { id = newId }));
            return Json(new { success = true });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}