using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Services.Impl
{
    public class WorkoutService : IWorkoutService
    {
        public required IRepository<Exercise> ExerciseRepository { protected get; init; }
        public required IRepository<Timing> TimingRepository { protected get; init; }
        public required IRandomize RandomizationService { protected get; init; }
        
        public WorkoutGenerationResponseModel Generate(WorkoutGenerationRequestModel request)
        {
            var random = new Random();
            var timings = TimingRepository.GetAll().ToList();
            var exercises = ExerciseRepository.GetAll().ToList();
            var equipment = exercises.Select(x => x.Equipment).Distinct().ToList();
            var addedExerciseIds = new List<long>();

            var timing = timings.FirstOrDefault(x => x.Name.ToLower() == request.Timing.ToLower());
            timing ??= RandomizationService.GetRandomItem(timings);

            var focus = request.Focus ?? RandomizationService.GetRandomItem(new[] { Models.Focus.Cardio, Models.Focus.Hybrid, Models.Focus.Strength });

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


            while(output.Exercises.Count < output.Stations)
            {
                var rand = random.NextDouble();
                Models.Focus exerciseFocus;
                if (rand < cardio)
                    exerciseFocus = Models.Focus.Cardio;
                else if (rand < strength)
                    exerciseFocus = Models.Focus.Strength;
                else
                    exerciseFocus = Models.Focus.Abs;

                var exerciseEquipment = equipment.OrderBy(x => random.NextDouble()).First();
                var exercise = RandomizationService.GetRandomItem(exercises.Where(x => x.FocusId == (byte)exerciseFocus && x.Equipment.ToLower() == exerciseEquipment.ToLower()));

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
