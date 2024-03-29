﻿using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Impl;
using WorkoutBuilder.Services.Tests.TestUtilities;

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
                var randomizer = A.Fake<IRandomize>();
                var exercises = new[]
                    {
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 2, Equipment = "Equipment 2", Name = "Exercise 2" },
                        new Exercise { Id = 3, Equipment = "Equipment 3", Name = "Exercise 3" }
                    };

                var exerciseRepository = new TestRepo<Exercise>(exercises);
                A.CallTo(() => randomizer.GetRandomItem<Models.Focus>(null)).WithAnyArguments().Returns(Models.Focus.Cardio);
                A.CallTo(() => randomizer.GetRandomItem<Exercise>(null)).WithAnyArguments()
                    .ReturnsNextFromSequence(exercises);

                A.CallTo(() => randomizer.NextDouble()).Returns(0);

                var workoutService = new GeneralWorkoutGenerator { ExerciseRepository = exerciseRepository, Randomizer = randomizer };
                var timing = new Timing { Id = 1, Name = "Fake Timing", Stations = 3, StationTiming = string.Empty };
                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel
                {
                    Timing = timing,
                    Equipment = new List<string> { "Equipment 1", "Equipment 2", "Equipment 3" }
                });

                Assert.IsNotNull(result);
                Assert.That(result.Exercises.Count, Is.EqualTo(3));
            }

            [Test]
            [TestCase(Models.Focus.Cardio)]
            [TestCase(Models.Focus.Strength)]
            [TestCase(Models.Focus.Hybrid)]
            public void Should_Generate_Requested_Focus(Models.Focus focus)
            {
                var randomizer = A.Fake<IRandomize>();
                var exercises = new[]
                    {
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 2, Equipment = "Equipment 2", Name = "Exercise 2" },
                        new Exercise { Id = 3, Equipment = "Equipment 3", Name = "Exercise 3" }
                    };

                var exerciseRepository = new TestRepo<Exercise>(exercises);
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
                var randomizer = A.Fake<IRandomize>();
                var exercises = new[]
                {
                        new Exercise { Id = 1, Equipment = "Equipment 1", Name = "Exercise 1" },
                        new Exercise { Id = 2, Equipment = "Equipment 2", Name = "Exercise 2" }
                };

                var exerciseRepository = new TestRepo<Exercise>(exercises);

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
                var result = workoutService.Generate(new Models.WorkoutGenerationRequestModel { Timing = timing, Equipment = new List<string> { "Equipment 1", "Equipment 2" } });

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
                var randomizer = A.Fake<IRandomize>();

                var exerciseRepository = new TestRepo<Exercise>();

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