using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Impl.Workout_Generators;

namespace WorkoutBuilder.Services.Impl
{
    public class WorkoutGeneratorFactory : IWorkoutGeneratorFactory
    {
        public IRepository<Timing> TimingRepository { protected get; init; }
        public IRandomize Randomizer { init; protected get; }
        public IRepository<Exercise> ExerciseRepository { init; protected get; }

        public Timing GetTiming(string timingName)
        {
            var timings = TimingRepository.GetAll().ToList();

            var timing = timings.FirstOrDefault(x => x.Name.Equals(timingName, StringComparison.OrdinalIgnoreCase));
            timing ??= Randomizer.GetRandomItem(timings);

            return timing;
        }

        public IWorkoutGenerator GetGenerator(Timing timing)
        {
            var defaultGenerator = new GeneralWorkoutGenerator { ExerciseRepository = ExerciseRepository, Randomizer = Randomizer };
            if (string.IsNullOrEmpty(timing.CustomGenerator))
                return defaultGenerator;

            switch(timing.CustomGenerator)
            {
                case nameof(MiamiNightsWorkoutGenerator):
                    return new MiamiNightsWorkoutGenerator { ExerciseRepository = ExerciseRepository, Randomizer = Randomizer };
                case nameof(ComboStationWorkoutGenerator):
                    return new ComboStationWorkoutGenerator { ExerciseRepository = ExerciseRepository, Randomizer = Randomizer };
                default:
                    return defaultGenerator;
            }

        }
    }
}
