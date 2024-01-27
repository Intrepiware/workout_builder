using Microsoft.AspNetCore.Mvc.Rendering;
using WorkoutBuilder.Data;
using WorkoutBuilder.Models;
using WorkoutBuilder.Models.ListItems;
using FocusEnum = WorkoutBuilder.Services.Models.Focus;

namespace WorkoutBuilder.Services
{
    public interface IExerciseModelMapper
    {
        ExercisesDetailsModel Map(Exercise exercise);
        Exercise Map(ExercisesDetailsModel model, Exercise exercise);

        ExerciseListItemModel MapList(Exercise? exercise);
    }

    public class ExerciseModelMapper : IExerciseModelMapper
    {
        public IRepository<Exercise> ExerciseRepository { init; protected get; } = null!;
        public IRepository<Part> PartsRepository { init; protected get; } = null!;
        public IUserContext UserContext { init; protected get; } = null!;
        public IUrlBuilder UrlBuilder { init; protected get; } = null!;

        public ExercisesDetailsModel Map(Exercise? exercise)
        {
            return new ExercisesDetailsModel
            {
                Equipment = exercise?.Equipment ?? string.Empty,
                FocusId = exercise?.FocusId ?? (long)(Enum.GetValues<FocusEnum>().First()),
                Id = exercise?.Id ?? 0,
                Name = exercise?.Name ?? string.Empty,
                Notes = exercise?.Notes,
                YoutubeUrl = exercise?.YoutubeUrl,
                FocusPartId = exercise?.FocusPartId,
                ActivationParts = exercise?.ExerciseParts?.Select(x => x.PartId).ToList() ?? new List<long>(),
                FocusOptions = Enum.GetValues<FocusEnum>().Select(x => new SelectListItem { Text = x.ToString(), Value = ((byte)x).ToString() }).ToList(),
                EquipmentOptions = ExerciseRepository.GetAll().Select(x => x.Equipment).Distinct().OrderBy(x => x)
                                            .Select(x => new SelectListItem { Value = x, Text = x }).ToList(),
                Parts = PartsRepository.GetAll().Select(x => new PartListItem { Id=  x.Id, Name = x.Name, IsMuscle = x.IsMuscle}).ToList()
            };
        }

        public ExerciseListItemModel MapList(Exercise exercise)
        {
            return new ExerciseListItemModel
            {
                EditUrl = UserContext.CanManageAllExercises() ? UrlBuilder.Action("Details", "Exercises", new { id = exercise.Id }) : null,
                Id = exercise.Id,
                Name = exercise.Name,
                Equipment = exercise.Equipment,
                Focus = ((FocusEnum)exercise.FocusId).ToString()
            };
        }

        public Exercise Map(ExercisesDetailsModel model, Exercise? exercise)
        {
            exercise ??= new Exercise();

            exercise.Name = model.Name;
            exercise.Notes = model.Notes;
            exercise.FocusId = model.FocusId;
            exercise.Equipment = model.Equipment;
            exercise.YoutubeUrl = model.YoutubeUrl;
            exercise.FocusPartId = model.FocusPartId;
            exercise.ExerciseParts = model.ActivationParts?.Select(x => new ExercisePart { PartId = x }).ToList();
            return exercise;
        }
    }
}