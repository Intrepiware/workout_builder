using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Impl;
using WorkoutBuilder.Services.Impl.Workout_Generators;
using WorkoutBuilder.Services.Tests.TestUtilities;

namespace WorkoutBuilder.Services.Tests
{
    public class MiamiNightsWorkoutGeneratorFixture
    {
        [TestFixture]
        public class When_Generating_Workout
        {
            [Test]
            public void Should_Generate()
            {
                var randomizer = A.Fake<IRandomize>();

                var exerciseRepository = new TestRepo<Exercise>();

                A.CallTo(() => randomizer.GetRandomItem<Models.Focus>(null)).WithAnyArguments().Returns(Models.Focus.Cardio);
                A.CallTo(() => randomizer.GetRandomItem<Exercise>(null)).WithAnyArguments()
                    .ReturnsNextFromSequence(Enumerable.Range(1, 12).Select(x => new Exercise { Equipment = $"Equipment {x}", Id = x, Name = $"Exercise {x}" }).ToArray());

                A.CallTo(() => randomizer.NextDouble()).Returns(0);

                var workoutService = new MiamiNightsWorkoutGenerator { ExerciseRepository = exerciseRepository, Randomizer = randomizer };

                var timing = new Timing { Id = 1, Name = "Fake Timing", Stations = 4, StationTiming = string.Empty };
                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel { Timing = timing });

                Assert.IsNotNull(result);
                Assert.That(result.Exercises.Count, Is.EqualTo(12));
            }

            [Test]
            public void Should_Not_Repeat_Exercise()
            {
                var randomizer = A.Fake<IRandomize>();
                var exerciseRepository = new TestRepo<Exercise>();

                A.CallTo(() => randomizer.GetRandomItem<Models.Focus>(null)).WithAnyArguments().Returns(Models.Focus.Cardio);

                var exercises = new List<Exercise>
                    {
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Duplicate - should not be included" },
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Duplicate - should not be included" },
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Duplicate - should not be included" },
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Duplicate - should not be included" }
                    };
                exercises.AddRange(Enumerable.Range(2, 11).Select(x => new Exercise { Equipment = $"Equipment {x}", Id = x, Name = $"Exercise {x}" }));
                A.CallTo(() => randomizer.GetRandomItem<Exercise>(null)).WithAnyArguments()
                    .ReturnsNextFromSequence(exercises.ToArray());

                A.CallTo(() => randomizer.NextDouble()).Returns(0);

                var workoutService = new MiamiNightsWorkoutGenerator { ExerciseRepository = exerciseRepository, Randomizer = randomizer };

                var timing = new Timing { Id = 1, Name = "Fake Timing", Stations = 4, StationTiming = string.Empty };
                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel { Timing = timing, Equipment = new List<string> { "Equipment 1" } });

                Assert.IsNotNull(result);
                Assert.That(result.Exercises.Count, Is.EqualTo(12));
                Assert.That(result.Exercises[0].Exercise, Is.EqualTo("Exercise 1"));
                Assert.That(result.Exercises[1].Exercise, Is.EqualTo("Exercise 2"));
                Assert.That(result.Exercises.Count(x => x.Exercise.Contains("duplicate")), Is.EqualTo(0));
            }

            [Test]
            public void Should_Generate_Cardio_Strength_Bodyweight_Pods()
            {
                // Create a list of 10 cardio, 10 strength, and 10 bodyweight exercises. Use those in IRepository<Exercise>
                var exercises = Enumerable.Range(1, 10)
                    .Select(x => new Exercise { Equipment = $"Equipment {x}", FocusId = (byte)Models.Focus.Cardio, Id = x, Name = $"Exercise {x}" }).ToList();
                exercises.AddRange(Enumerable.Range(11, 10)
                    .Select(x => new Exercise { Equipment = $"Equipment {x}", FocusId = (byte)Models.Focus.Strength, Id = x, Name = $"Exercise {x}" }));
                exercises.AddRange(Enumerable.Range(21, 10)
                    .Select(x => new Exercise { Equipment = $"Bodyweight", FocusId = (byte)Models.Focus.Abs, Id = x, Name = $"Exercise {x}" }));

                var randomizer = new RandomizeService();

                var exerciseRepository = new TestRepo<Exercise>(exercises);

                var workoutService = new MiamiNightsWorkoutGenerator { ExerciseRepository = exerciseRepository, Randomizer = randomizer };

                var timing = new Timing { Id = 1, Name = "Fake Timing", Stations = 12, StationTiming = string.Empty };
                var equipment = Enumerable.Range(1, 20).Select(x => $"Equipment {x}").Append("Bodyweight").ToList();
                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel { Timing = timing, Equipment = equipment });

                Assert.That(result.Exercises.Count, Is.EqualTo(12));
                Assert.That(result.Exercises.Where((x, i) => i < 4 && x.Focus == "Strength").Count(), Is.EqualTo(0), "The first four elements should be cardio or abs");
                Assert.That(result.Exercises.Where((x, i) => i >= 4 && i < 8 && x.Focus == "Cardio").Count(), Is.EqualTo(0), "The middle four elements should be strength or abs");
                Assert.That(result.Exercises.Where((x, i) => i >= 8 && x.Equipment != "Bodyweight").Count(), Is.EqualTo(0), "The last four elements should be bodyweight");
            }
        }

        [TestFixture]
        public class When_Insufficent_Exercises
        {
            [Test]
            // Tests to make sure that the program does not get stuck in an infinite loop
            public void Should_Generate()
            {
                var randomizer = A.Fake<IRandomize>();

                var exerciseRepository = new TestRepo<Exercise>();

                A.CallTo(() => randomizer.GetRandomItem<Models.Focus>(null)).WithAnyArguments().Returns(Models.Focus.Cardio);
                A.CallTo(() => randomizer.GetRandomItem<Exercise>(null)).WithAnyArguments().Returns(null);

                A.CallTo(() => randomizer.NextDouble()).Returns(0);

                var workoutService = new MiamiNightsWorkoutGenerator { ExerciseRepository = exerciseRepository, Randomizer = randomizer };

                var timing = new Timing { Id = 1, Name = "Fake Timing", Stations = 3, StationTiming = string.Empty };
                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel { Timing = timing });

                Assert.IsNotNull(result);
                Assert.That(result.Exercises.Count, Is.EqualTo(0));
            }
        }
    }
}