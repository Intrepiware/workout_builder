using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Services.Impl.Workout_Generators
{
    public class MiamiNightsWorkoutGenerator : IWorkoutGenerator
    {
        public IRandomize Randomizer { init; protected get; }
        public IRepository<Exercise> ExerciseRepository { init; protected get; }

        public WorkoutGenerationResponseModel Generate(WorkoutGenerationRequestModel request)
        {
            var exercises = ExerciseRepository.GetAll().ToList();
            var equipment = exercises.Select(x => x.Equipment)
                                    .Where(x => !x.Equals("bodyweight", StringComparison.OrdinalIgnoreCase))
                                    .Distinct()
                                    .ToList();
            var addedExerciseIds = new List<long>();
            const int MaxIterations = 1000;

            var timing = request.Timing;
            var output = new WorkoutGenerationResponseModel
            {
                Focus = "Hybrid",
                Name = timing.Name,
                Notes = timing.Notes,
                Stations = timing.Stations,
                Timing = timing.StationTiming,
                Exercises = new List<WorkoutGenerationExerciseModel>()
            };

            var iterations = 0;
            var addedExercises = new List<Exercise>();

            // First, add 4 cardio stations
            while (addedExercises.Count < 4 && iterations++ < MaxIterations)
            {
                var rand = Randomizer.NextDouble();
                var exerciseFocus = (rand < .8) ? Models.Focus.Cardio : Models.Focus.Abs;

                var exercise = Randomizer.GetRandomItem(exercises.Where(x => x.FocusId == (byte)exerciseFocus && !x.Equipment.Equals("bodyweight", StringComparison.OrdinalIgnoreCase)));

                if (exercise != null && !addedExercises.Any(x => x.Id == exercise.Id))
                    addedExercises.Add(exercise);
            }

            // Next add 4 strength stations
            while (addedExercises.Count < 8 && iterations++ < MaxIterations)
            {
                var rand = Randomizer.NextDouble();
                var exerciseFocus = (rand < .8) ? Models.Focus.Strength : Models.Focus.Abs;

                var exercise = Randomizer.GetRandomItem(exercises.Where(x => x.FocusId == (byte)exerciseFocus && !x.Equipment.Equals("bodyweight", StringComparison.OrdinalIgnoreCase)));

                if (exercise != null && !addedExercises.Any(x => x.Id == exercise.Id))
                    addedExercises.Add(exercise);
            }

            // Finally add 4 bodyweight stations
            while (addedExercises.Count < 12 && iterations++ < MaxIterations)
            {
                var rand = Randomizer.NextDouble();
                var exercise = Randomizer.GetRandomItem(exercises.Where(x => x.Equipment.Equals("bodyweight", StringComparison.OrdinalIgnoreCase)));
                if (exercise != null && !addedExercises.Any(x => x.Id == exercise.Id))
                    addedExercises.Add(exercise);
            }

            output.Exercises = addedExercises.Select((x, i) => new WorkoutGenerationExerciseModel
            {
                Equipment = x.Equipment,
                Exercise = x.Name,
                Focus = ((Models.Focus)x.FocusId).ToString(),
                Notes = x.Notes,
                Station = $"{i + 1}"
            }).ToList();

            return output;
        }
    }
}