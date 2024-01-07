using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using WorkoutBuilder.Data;
using WorkoutBuilder.Services;
using WorkoutBuilder.Services.ExtensionMethods;

namespace WorkoutBuilder.Controllers
{
    [Authorize]
    public class ExercisesController : Controller
    {
        public IRepository<Exercise> ExerciseRepository { init; protected get; } = null!;
        public IUserContext UserContext { init; protected get; } = null!;
        public IExerciseService ExerciseService { init; protected get; } = null!;
        public IExerciseModelMapper ExerciseModelMapper { init; protected get; } = null!;

        public IActionResult Index(int take, int skip, string? name = null, string? focus = null, string? equipment = null)
        {
            if (!UserContext.CanReadAllExercises())
                throw new SecurityException();

            if(Request.IsAjaxRequest())
            {
                var canManageExercises = UserContext.CanManageAllExercises();
                var output = ExerciseService.Search(take, skip, name, focus, equipment)
                                .Select(ExerciseModelMapper.MapList)
                                .ToList();

                return Json(output);
            }

            return View();
        }


        [Route("[controller]/[action]/{id}")]
        public async Task<IActionResult> Index(long id)
        {
            if (!UserContext.CanManageAllExercises())
                throw new SecurityException();

            var exercise = await ExerciseRepository.GetById(id);
            if (exercise == null)
                return NotFound();

            var model = ExerciseModelMapper.Map(exercise);
            return View(model);
        }
    }
}
