using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Services.Impl
{
    public class WorkoutService : IWorkoutService
    {
        public required IRepository<Exercise> ExerciseRepository { protected get; init; }
        public required IRepository<Timing> TimingRepository { protected get; init; }
        public required IRandomize Randomizer { protected get; init; }
        
        public WorkoutGenerationResponseModel Generate(WorkoutGenerationRequestModel request)
        {
            var timings = TimingRepository.GetAll().ToList();
            var exercises = ExerciseRepository.GetAll().ToList();
            var equipment = exercises.Select(x => x.Equipment).Distinct().ToList();
            var addedExerciseIds = new List<long>();
            const int MaxIterations = 1000;

            var timing = timings.FirstOrDefault(x => x.Name.Equals(request.Timing, StringComparison.OrdinalIgnoreCase));
            timing ??= Randomizer.GetRandomItem(timings);

            var focus = request.Focus ?? Randomizer.GetRandomItem(new[] { Models.Focus.Cardio, Models.Focus.Hybrid, Models.Focus.Strength });

            double cardio = 0, strength = .8;

            switch(request.Focus)
            {
                case Models.Focus.Strength:
                    cardio = 0.2;
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
                Focus = focus,
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

                var exerciseEquipment = Randomizer.GetRandomItem(equipment);
                var exercise = Randomizer.GetRandomItem(exercises.Where(x => x.FocusId == (byte)exerciseFocus && x.Equipment.Equals(exerciseEquipment, StringComparison.OrdinalIgnoreCase)));

                if(exercise != null && !addedExerciseIds.Contains(exercise.Id))
                {
                    output.Exercises.Add(new WorkoutGenerationExerciseModel
                    {
                        Equipment = exercise.Equipment,
                        Exercise = exercise.Name,
                        Focus = exerciseFocus.ToString(),
                        Notes = exercise.Notes,
                        Station = output.Exercises.Count + 1
                    });
                    addedExerciseIds.Add(exercise.Id);
                }
            }
            return output;
        }
    }
}
