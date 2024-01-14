using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Services.Impl
{
    public class GeneralWorkoutGenerator : IWorkoutGenerator
    {
        public IRepository<Exercise> ExerciseRepository { protected get; init; } = null!;
        public IRandomize Randomizer { protected get; init; } = null!;

        public WorkoutGenerationResponseModel Generate(WorkoutGenerationRequestModel request)
        {
            var equipment = request.Equipment ?? new List<string>();
            var exercises = ExerciseRepository.GetAll().Where(x => equipment.Contains(x.Equipment)).ToList();
            var allEquipment = exercises.Select(x => x.Equipment).Distinct().ToList();
            var addedExerciseIds = new List<long>();
            const int MaxIterations = 1000;
            
            var timing = request.Timing;
            var focus = request.Focus ?? Randomizer.GetRandomItem(new[] { Models.Focus.Cardio, Models.Focus.Hybrid, Models.Focus.Strength });

            double cardio = 0, strength = .8;

            switch(focus)
            {
                case Models.Focus.Strength:
                    cardio = 0;
                    break;
                case Models.Focus.Hybrid:
                    cardio = 0.4;
                    break;
                case Models.Focus.Cardio:
                default:
                    cardio = 0.6;
                    break;

            }

            var output = new WorkoutGenerationResponseModel
            {
                Focus = focus.ToString(),
                Name = timing.Name,
                Notes = timing.Notes,
                Stations = timing.Stations,
                Timing = timing.StationTiming,
                Exercises = new List<WorkoutGenerationExerciseModel>()
            };

            var iterations = 0;
            while(output.Exercises.Count < output.Stations && iterations++ < MaxIterations)
            {
                var rand = Randomizer.NextDouble();
                Models.Focus exerciseFocus;
                if (rand < cardio)
                    exerciseFocus = Models.Focus.Cardio;
                else if (rand < strength)
                    exerciseFocus = Models.Focus.Strength;
                else
                    exerciseFocus = Models.Focus.Abs;

                // Don't use more than 15 different pieces of equipment per workout
                var usedEquipment = output.Exercises.Select(x => x.Equipment).Distinct();
                var allowedEquipment = usedEquipment.Count() >= 15 ? usedEquipment.ToList() : allEquipment.ToList();
                var exercise = Randomizer.GetRandomItem(exercises.Where(x => x.FocusId == (byte)exerciseFocus));

                if(exercise != null && allowedEquipment.Contains(exercise.Equipment) && !addedExerciseIds.Contains(exercise.Id))
                {
                    output.Exercises.Add(new WorkoutGenerationExerciseModel
                    {
                        Equipment = exercise.Equipment,
                        Exercise = exercise.Name,
                        YoutubeUrl = exercise.YoutubeUrl,
                        Focus = exerciseFocus.ToString(),
                        Notes = exercise.Notes,
                        Station = $"{output.Exercises.Count + 1}"
                    });
                    addedExerciseIds.Add(exercise.Id);
                }
            }
            return output;
        }
    }
}
