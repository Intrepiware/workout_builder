using FakeItEasy;
using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Impl;

namespace WorkoutBuilder.Services.Tests
{
    public class GeneralWorkoutGeneratorFixture
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
                        new Exercise { Id = 2, Equipment = "Equipment 2", Name = "Exercise 2" },
                        new Exercise { Id = 3, Equipment = "Equipment 3", Name = "Exercise 3" }
                    };

                A.CallTo(() => exerciseRepository.GetAll()).Returns(exercises.AsQueryable());
                A.CallTo(() => randomizer.GetRandomItem<Models.Focus>(null)).WithAnyArguments().Returns(Models.Focus.Cardio);
                A.CallTo(() => randomizer.GetRandomItem<Exercise>(null)).WithAnyArguments()
                    .ReturnsNextFromSequence(exercises);

                A.CallTo(() => randomizer.NextDouble()).Returns(0);

                var workoutService = new GeneralWorkoutGenerator { ExerciseRepository = exerciseRepository, Randomizer = randomizer };
                var timing = new Timing { Id = 1, Name = "Fake Timing", Stations = 3, StationTiming = string.Empty };
                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel { Timing = timing });

                Assert.IsNotNull(result);
                Assert.That(result.Exercises.Count, Is.EqualTo(3));
            }

            [Test]
            [TestCase(Models.Focus.Cardio)]
            [TestCase(Models.Focus.Strength)]
            [TestCase(Models.Focus.Hybrid)]
            public void Should_Generate_Requested_Focus(Models.Focus focus)
            {
                var exerciseRepository = A.Fake<IRepository<Exercise>>();
                var randomizer = A.Fake<IRandomize>();
                var exercises = new[]
                    {
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 2, Equipment = "Equipment 2", Name = "Exercise 2" },
                        new Exercise { Id = 3, Equipment = "Equipment 3", Name = "Exercise 3" }
                    };

                A.CallTo(() => exerciseRepository.GetAll()).Returns(exercises.AsQueryable());
                A.CallTo(() => randomizer.GetRandomItem<Models.Focus>(null)).WithAnyArguments().Returns(Models.Focus.Cardio);
                A.CallTo(() => randomizer.GetRandomItem<Exercise>(null)).WithAnyArguments()
                    .ReturnsNextFromSequence(exercises);

                A.CallTo(() => randomizer.NextDouble()).Returns(0);

                var workoutService = new GeneralWorkoutGenerator { ExerciseRepository = exerciseRepository, Randomizer = randomizer };

                var timing = new Timing { Id = 1, Name = "Fake Timing", Stations = 3, StationTiming = string.Empty };
                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel { Focus = focus, Timing = timing });

                Assert.IsNotNull(result);
                Assert.That(result.Focus, Is.EqualTo(focus.ToString()));
            }

            [Test]
            public void Should_Not_Repeat_Exercise()
            {
                var exerciseRepository = A.Fake<IRepository<Exercise>>();
                var randomizer = A.Fake<IRandomize>();
                var exercises = new[]
                {
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 2, Equipment = "Equipment 2", Name = "Exercise 2" }
                };

                A.CallTo(() => exerciseRepository.GetAll()).Returns(exercises.AsQueryable());

                A.CallTo(() => randomizer.GetRandomItem<Models.Focus>(null)).WithAnyArguments().Returns(Models.Focus.Cardio);
                A.CallTo(() => randomizer.GetRandomItem<Exercise>(null)).WithAnyArguments()
                    .ReturnsNextFromSequence(new[]
                    {
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 2, Equipment = "Equipment 2", Name = "Exercise 2" }
                    });

                A.CallTo(() => randomizer.NextDouble()).Returns(0);

                var workoutService = new GeneralWorkoutGenerator { ExerciseRepository = exerciseRepository, Randomizer = randomizer };

                var timing = new Timing { Id = 1, Name = "Fake Timing", Stations = 2, StationTiming = string.Empty };
                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel { Timing = timing });

                Assert.IsNotNull(result);
                Assert.That(result.Exercises[0].Exercise, Is.EqualTo("Exercise 1"));
                Assert.That(result.Exercises[1].Exercise, Is.EqualTo("Exercise 2"));
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

                var workoutService = new GeneralWorkoutGenerator { ExerciseRepository = exerciseRepository, Randomizer = randomizer };

                var timing = new Timing { Id = 1, Name = "Fake Timing", Stations = 3, StationTiming = string.Empty };
                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel { Timing = timing });

                Assert.IsNotNull(result);
                Assert.That(result.Exercises.Count, Is.EqualTo(0));
            }
        }
    }
}