﻿using Microsoft.AspNetCore.Mvc.Rendering;
using WorkoutBuilder.Data;
using WorkoutBuilder.Models;
using WorkoutBuilder.Models.ListItems;
using FocusEnum = WorkoutBuilder.Services.Models.Focus;

namespace WorkoutBuilder.Services
{
    public interface IExerciseModelMapper
    {
        ExercisesDetailsModel Map(Exercise exercise);

        ExerciseListItemModel MapList(Exercise exercise);
    }

    public class ExerciseModelMapper : IExerciseModelMapper
    {
        public IRepository<Exercise> ExerciseRepository { init; protected get; } = null!;
        public IUserContext UserContext { init; protected get; } = null!;
        public IUrlBuilder UrlBuilder { init; protected get; } = null!;

        public ExercisesDetailsModel Map(Exercise exercise)
        {
            return new ExercisesDetailsModel
            {
                Equipment = exercise.Equipment,
                FocusId = exercise.FocusId,
                Id = exercise.Id,
                Name = exercise.Name,
                Notes = exercise.Notes,
                FocusOptions = Enum.GetValues<FocusEnum>().Select(x => new SelectListItem { Text = x.ToString(), Value = ((byte)x).ToString() }).ToList(),
                EquipmentOptions = ExerciseRepository.GetAll().Select(x => x.Equipment).Distinct().OrderBy(x => x)
                                            .Select(x => new SelectListItem { Value = x, Text = x }).ToList()
            };
        }

        public ExerciseListItemModel MapList(Exercise exercise)
        {
            return new ExerciseListItemModel
            {
                EditUrl = UserContext.CanManageAllExercises() ? UrlBuilder.Action("Details", "Exercises", new { id = exercise.Id }) : null,
                Id = exercise.Id,
                Name = exercise.Name,
                Focus = ((FocusEnum)exercise.FocusId).ToString()
            };
        }
    }
}