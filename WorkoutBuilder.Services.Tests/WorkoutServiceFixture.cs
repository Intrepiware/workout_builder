using FakeItEasy;
using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Impl;

namespace WorkoutBuilder.Services.Tests
{
    public class WorkoutServiceFixture
    {
        [TestFixture]
        public class When_Generating_Workout
        {
            [Test]
            public void Should_Generate()
            {
                var exerciseRepository = A.Fake<IRepository<Exercise>>();
                var timingRepository = A.Fake<IRepository<Timing>>();
                var randomizer = A.Fake<IRandomize>();


                A.CallTo(() => exerciseRepository.GetAll()).Returns(new List<Exercise>().AsQueryable());
                A.CallTo(() => timingRepository.GetAll()).Returns(new List<Timing>().AsQueryable());

                A.CallTo(() => randomizer.GetRandomItem<Timing>(null)).WithAnyArguments().Returns(new Timing { Id = 1, Name = "Fake Timing", Stations = 3, StationTiming = string.Empty });
                A.CallTo(() => randomizer.GetRandomItem<Models.Focus>(null)).WithAnyArguments().Returns(Models.Focus.Cardio);
                A.CallTo(() => randomizer.GetRandomItem<Exercise>(null)).WithAnyArguments()
                    .ReturnsNextFromSequence(new[]
                    {
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 2, Equipment = "Equipment 2", Name = "Exercise 2" },
                        new Exercise { Id = 3, Equipment = "Equipment 3", Name = "Exercise 3" }
                    });

                A.CallTo(() => randomizer.NextDouble()).Returns(0);

                var workoutService = new WorkoutService { ExerciseRepository = exerciseRepository, Randomizer = randomizer, TimingRepository = timingRepository };

                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel());

                Assert.IsNotNull(result);
            }

            [Test]
            [TestCase(Models.Focus.Cardio)]
            [TestCase(Models.Focus.Strength)]
            [TestCase(Models.Focus.Hybrid)]
            public void Should_Generate_Requested_Focus(Models.Focus focus)
            {
                var exerciseRepository = A.Fake<IRepository<Exercise>>();
                var timingRepository = A.Fake<IRepository<Timing>>();
                var randomizer = A.Fake<IRandomize>();


                A.CallTo(() => exerciseRepository.GetAll()).Returns(new List<Exercise>().AsQueryable());
                A.CallTo(() => timingRepository.GetAll()).Returns(new List<Timing>().AsQueryable());

                A.CallTo(() => randomizer.GetRandomItem<Timing>(null)).WithAnyArguments().Returns(new Timing { Id = 1, Name = "Fake Timing", Stations = 3, StationTiming = string.Empty });
                A.CallTo(() => randomizer.GetRandomItem<Models.Focus>(null)).WithAnyArguments().Returns(Models.Focus.Cardio);
                A.CallTo(() => randomizer.GetRandomItem<Exercise>(null)).WithAnyArguments()
                    .ReturnsNextFromSequence(new[]
                    {
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 2, Equipment = "Equipment 2", Name = "Exercise 2" },
                        new Exercise { Id = 3, Equipment = "Equipment 3", Name = "Exercise 3" }
                    });

                A.CallTo(() => randomizer.NextDouble()).Returns(0);

                var workoutService = new WorkoutService { ExerciseRepository = exerciseRepository, Randomizer = randomizer, TimingRepository = timingRepository };

                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel { Focus = focus });

                Assert.IsNotNull(result);
                Assert.That(result.Focus, Is.EqualTo(focus));
            }

            [Test]
            public void Should_Generate_Requested_Timing()
            {
                var exerciseRepository = A.Fake<IRepository<Exercise>>();
                var timingRepository = A.Fake<IRepository<Timing>>();
                var randomizer = A.Fake<IRandomize>();


                A.CallTo(() => exerciseRepository.GetAll()).Returns(new List<Exercise>().AsQueryable());
                A.CallTo(() => timingRepository.GetAll()).Returns(new List<Timing> {
                    new Timing { Id = 1, Name = "First Workout", Stations = 1 },
                    new Timing { Id = 2, Name = "Second Workout", Stations = 1 },
                }.AsQueryable());

                A.CallTo(() => randomizer.GetRandomItem<Timing>(null)).WithAnyArguments().Returns(new Timing { Id = 1, Name = "Fake Workout", Stations = 3, StationTiming = string.Empty });
                A.CallTo(() => randomizer.GetRandomItem<Models.Focus>(null)).WithAnyArguments().Returns(Models.Focus.Cardio);
                A.CallTo(() => randomizer.GetRandomItem<Exercise>(null)).WithAnyArguments()
                    .ReturnsNextFromSequence(new[]
                    {
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 2, Equipment = "Equipment 2", Name = "Exercise 2" },
                        new Exercise { Id = 3, Equipment = "Equipment 3", Name = "Exercise 3" }
                    });

                A.CallTo(() => randomizer.NextDouble()).Returns(0);

                var workoutService = new WorkoutService { ExerciseRepository = exerciseRepository, Randomizer = randomizer, TimingRepository = timingRepository };

                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel { Timing = "Second Workout" });

                Assert.IsNotNull(result);
                Assert.That(result.Name, Is.EqualTo("Second Workout"));
            }

            [Test]
            public void Should_Generate_NonExistent_Timing()
            {
                var exerciseRepository = A.Fake<IRepository<Exercise>>();
                var timingRepository = A.Fake<IRepository<Timing>>();
                var randomizer = A.Fake<IRandomize>();


                A.CallTo(() => exerciseRepository.GetAll()).Returns(new List<Exercise>().AsQueryable());
                A.CallTo(() => timingRepository.GetAll()).Returns(new List<Timing> {
                    new Timing { Id = 1, Name = "First Workout", Stations = 1 },
                    new Timing { Id = 2, Name = "Second Workout", Stations = 1 },
                }.AsQueryable());

                A.CallTo(() => randomizer.GetRandomItem<Timing>(null)).WithAnyArguments().Returns(new Timing { Id = 1, Name = "Random Workout", Stations = 3, StationTiming = string.Empty });
                A.CallTo(() => randomizer.GetRandomItem<Models.Focus>(null)).WithAnyArguments().Returns(Models.Focus.Cardio);
                A.CallTo(() => randomizer.GetRandomItem<Exercise>(null)).WithAnyArguments()
                    .ReturnsNextFromSequence(new[]
                    {
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 2, Equipment = "Equipment 2", Name = "Exercise 2" },
                        new Exercise { Id = 3, Equipment = "Equipment 3", Name = "Exercise 3" }
                    });

                A.CallTo(() => randomizer.NextDouble()).Returns(0);

                var workoutService = new WorkoutService { ExerciseRepository = exerciseRepository, Randomizer = randomizer, TimingRepository = timingRepository };

                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel { Timing = "Non-Existent Workout" });

                Assert.IsNotNull(result);
                Assert.That(result.Name, Is.EqualTo("Random Workout"));
            }

        }

        [TestFixture]
        public class When_Insufficent_Exercises
        {
            [Test]
            // Tests to make sure that the program does not get stuck in an infinite loop
            public void Should_Generate()
            {
                var exerciseRepository = A.Fake<IRepository<Exercise>>();
                var timingRepository = A.Fake<IRepository<Timing>>();
                var randomizer = A.Fake<IRandomize>();

                A.CallTo(() => exerciseRepository.GetAll()).Returns(new List<Exercise>().AsQueryable());
                A.CallTo(() => timingRepository.GetAll()).Returns(new List<Timing>().AsQueryable());

                A.CallTo(() => randomizer.GetRandomItem<Timing>(null)).WithAnyArguments().Returns(new Timing { Id = 1, Name = "Fake Timing", Stations = 3, StationTiming = string.Empty });
                A.CallTo(() => randomizer.GetRandomItem<Models.Focus>(null)).WithAnyArguments().Returns(Models.Focus.Cardio);
                A.CallTo(() => randomizer.GetRandomItem<Exercise>(null)).WithAnyArguments().Returns(null);

                A.CallTo(() => randomizer.NextDouble()).Returns(0);

                var workoutService = new WorkoutService { ExerciseRepository = exerciseRepository, Randomizer = randomizer, TimingRepository = timingRepository };

                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel());

                Assert.IsNotNull(result);
                Assert.That(result.Exercises.Count, Is.EqualTo(0));
            }
        }
    }
}
