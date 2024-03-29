﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security;
using WorkoutBuilder.Data;
using WorkoutBuilder.Models;
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
                return Unauthorized();

            if (Request.IsAjaxRequest())
            {
                var output = ExerciseService.Search(take, skip, name, focus, equipment)
                                .Select(ExerciseModelMapper.MapList)
                                .ToList();

                return Json(output);
            }

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            if (!UserContext.CanManageAllExercises())
                return Unauthorized();

            var exercise = await ExerciseRepository.GetById(id);
            if (exercise == null)
                return NotFound();

            var model = ExerciseModelMapper.Map(exercise);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ExercisesDetailsModel model)
        {
            if (!UserContext.CanManageAllExercises())
                return Unauthorized();

            var exercise = ExerciseRepository.GetAll().Include(x => x.ExerciseParts).SingleOrDefault(x => x.Id == model.Id);
            if (exercise == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            exercise = ExerciseModelMapper.Map(model, exercise);
            await ExerciseService.Update(exercise);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult New()
        {
            if (!UserContext.CanManageAllExercises())
                return Unauthorized();
            var model = ExerciseModelMapper.Map(null!);
            return View("Details", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(ExercisesDetailsModel model)
        {
            if (!UserContext.CanManageAllExercises())
                return Unauthorized();

            if (!ModelState.IsValid)
                return View(model);

            var exercise = ExerciseModelMapper.Map(model, null!);

            await ExerciseService.Add(exercise);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long exerciseId)
        {
            if (!UserContext.CanManageAllExercises())
                return Unauthorized();

            await ExerciseService.Delete(exerciseId);
            return RedirectToAction("Index");
        }
    }
}
