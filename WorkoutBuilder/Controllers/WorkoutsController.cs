using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WorkoutBuilder.Data;
using WorkoutBuilder.Models;
using WorkoutBuilder.Services;
using WorkoutBuilder.Services.ExtensionMethods;
using WorkoutBuilder.Services.Impl;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Controllers
{
    [Authorize]
    public class WorkoutsController : Controller
    {
        public IRepository<Workout> WorkoutRepository { init; protected get; } = null!;
        public IUserContext UserContext { init; protected get; } = null!;
        public IWorkoutService WorkoutService { protected get; init; } = null!;
        public IUrlBuilder UrlBuilder { protected get; init; } = null!;

        public IActionResult Index(int take = 25, int skip = 0, bool onlyFavorites = false)
        {
            if(Request.IsAjaxRequest())
            {
                var query = WorkoutRepository.GetAll().Where(x => x.UserId == UserContext.GetUserId().Value);

                if (onlyFavorites)
                    query = query.Where(x => x.IsFavorite);

                var output = query
                                .OrderByDescending(x => x.CreateDate)
                                .Skip(skip).Take(take)
                                .Select(x => new WorkoutListItemModel
                                {
                                    CreateDate = x.CreateDate,
                                    Id = x.Id,
                                    IsFavorite = x.IsFavorite,
                                    Name = JsonConvert.DeserializeObject<WorkoutGenerationResponseModel>(x.Body).Name,
                                    PublicId = x.PublicId
                                })
                                .ToList();

                return Json(output);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Favorite(string id)
        {
            var workout = await WorkoutService.ToggleFavorite(id);
            if (workout != null && workout.PublicId != id)
                Response.Headers.Add("Location", UrlBuilder.Action("Index", "Home", new { id = workout.PublicId }));
            return Json(new { success = true, workout.IsFavorite });
        }
    }
}
