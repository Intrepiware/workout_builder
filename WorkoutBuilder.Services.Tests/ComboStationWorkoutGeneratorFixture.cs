using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Impl.Workout_Generators;

namespace WorkoutBuilder.Services.Tests
{
    public class ComboStationWorkoutGeneratorFixture
    {
        [TestFixture]
        public class When_Generating_Workout
        {
            [Test]
            public void Should_Generate()
            {
                var exerciseRepository = A.Fake<IRepository<Exercise>>();
                var randomizer = A.Fake<IRandomize>();
                var exercises = new[]
                    {
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 2, Equipment = "Equipment 1", Name = "Exercise 2" },
                        new Exercise { Id = 3, Equipment = "Equipment 2", Name = "Exercise 3" },
                        new Exercise { Id = 4, Equipment = "Equipment 2", Name = "Exercise 4" }
                    };

                A.CallTo(() => exerciseRepository.GetAll()).Returns(exercises.AsQueryable());
                A.CallTo(() => randomizer.GetRandomItem<Models.Focus>(null)).WithAnyArguments().Returns(Models.Focus.Cardio);
                A.CallTo(() => randomizer.GetRandomItem<Exercise>(null)).WithAnyArguments()
                    .ReturnsNextFromSequence(exercises);

                A.CallTo(() => randomizer.NextDouble()).Returns(0);

                var workoutService = new ComboStationWorkoutGenerator { ExerciseRepository = exerciseRepository, Randomizer = randomizer };

                var timing = new Timing { Id = 1, Name = "Fake Timing", Stations = 4, StationTiming = string.Empty };
                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel { Timing = timing, Equipment = new List<string> { "Equipment 1", "Equipment 2" } });

                Assert.IsNotNull(result);
                Assert.That(result.Exercises.Count, Is.EqualTo(4));
                Assert.That(result.Exercises[0].Station, Is.EqualTo("1A"));
                Assert.That(result.Exercises[1].Station, Is.EqualTo("1B"));
                Assert.That(result.Exercises[2].Station, Is.EqualTo("2A"));
                Assert.That(result.Exercises[3].Station, Is.EqualTo("2B"));
            }

            [Test]
            public void Should_Not_Repeat_Exercise()
            {
                var exerciseRepository = A.Fake<IRepository<Exercise>>();
                var randomizer = A.Fake<IRandomize>();
                var exercises = new[]
                    {
                        // Pair should be included
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 2, Equipment = "Equipment 1", Name = "Exercise 2" },

                        // Pair should not be included
                        new Exercise { Id = 3, Equipment = "Equipment 1", Name = "Same exercise as below - should not be included" },
                        new Exercise { Id = 3, Equipment = "Equipment 1", Name = "Same exercise as above - should not be included" },

                        // Pair should not be included
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Same exercise as first - should not be included" },
                        new Exercise { Id = 4, Equipment = "Equipment 1", Name = "Exercise 4" },

                        // Pair should not be included
                        new Exercise { Id = 5, Equipment = "Equipment 1", Name = "Exercise 5" },
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Same exercise as first - should not be included" },

                        // Pair should be included
                        new Exercise { Id = 6, Equipment = "Equipment 1", Name = "Exercise 6" },
                        new Exercise { Id = 7, Equipment = "Equipment 1", Name = "Exercise 7" }
                    };

                A.CallTo(() => exerciseRepository.GetAll()).Returns(exercises.AsQueryable());

                A.CallTo(() => randomizer.GetRandomItem<Models.Focus>(null)).WithAnyArguments().Returns(Models.Focus.Cardio);
                A.CallTo(() => randomizer.GetRandomItem<Exercise>(null)).WithAnyArguments()
                    .ReturnsNextFromSequence(exercises);

                A.CallTo(() => randomizer.NextDouble()).Returns(0);

                var workoutService = new ComboStationWorkoutGenerator { ExerciseRepository = exerciseRepository, Randomizer = randomizer };

                var timing = new Timing { Id = 1, Name = "Fake Timing", Stations = 4, StationTiming = string.Empty };
                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel { Timing = timing, Equipment = new List<string> { "Equipment 1" } });

                Assert.IsNotNull(result);
                Assert.That(result.Exercises.Count, Is.EqualTo(4));
                Assert.That(result.Exercises[0].Exercise, Is.EqualTo("Exercise 1"));
                Assert.That(result.Exercises[1].Exercise, Is.EqualTo("Exercise 2"));
                Assert.That(result.Exercises[2].Exercise, Is.EqualTo("Exercise 6"));
                Assert.That(result.Exercises[3].Exercise, Is.EqualTo("Exercise 7"));
            }
        }

        [TestFixture]
        public class When_Insufficent_Exercises
        {
            [Test]
            public void Should_Generate()
            {
                var exerciseRepository = A.Fake<IRepository<Exercise>>();
                var randomizer = A.Fake<IRandomize>();

                A.CallTo(() => exerciseRepository.GetAll()).Returns(new List<Exercise>().AsQueryable());

                A.CallTo(() => randomizer.GetRandomItem<Models.Focus>(null)).WithAnyArguments().Returns(Models.Focus.Cardio);
                A.CallTo(() => randomizer.GetRandomItem<Exercise>(null)).WithAnyArguments().Returns(null);

                A.CallTo(() => randomizer.NextDouble()).Returns(0);

                var workoutService = new ComboStationWorkoutGenerator { ExerciseRepository = exerciseRepository, Randomizer = randomizer };

                var timing = new Timing { Id = 1, Name = "Fake Timing", Stations = 3, StationTiming = string.Empty };
                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel { Timing = timing });

                Assert.IsNotNull(result);
                Assert.That(result.Exercises.Count, Is.EqualTo(0));
            }
        }
    }
}