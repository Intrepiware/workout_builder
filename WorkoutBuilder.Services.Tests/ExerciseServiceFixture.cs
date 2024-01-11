using System.Security;
using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Impl;
using WorkoutBuilder.Services.Tests.TestUtilities;

namespace WorkoutBuilder.Services.Tests
{
    public class ExerciseServiceFixture
    {
        [TestFixture]
        public class When_Adding_Exercise
        {
            [Test]
            public async Task Should_Add()
            {
                // Arrange
                var userContext = A.Fake<IUserContext>();
                var exerciseRepository = new TestRepo<Exercise>(new[] { new Exercise
                {
                    Equipment = "dumbbell",
                    Name = "Curls"
                } });
                A.CallTo(() => userContext.CanManageAllExercises()).Returns(true);
                var newExercise = new Exercise { Equipment = "dumbbell", Name = "New Exercise", FocusId = (byte)Models.Focus.Strength };
                var exerciseService = new ExerciseService { ExerciseRepository = exerciseRepository, UserContext = userContext };

                // Act
                await exerciseService.Add(newExercise);

                // Assert
                Assert.That(exerciseRepository.AddedItems.Count, Is.EqualTo(1));
            }

            [Test]
            public void Should_Not_Add_New_Equipment()
            {
                // Arrange
                var userContext = A.Fake<IUserContext>();
                var exerciseRepository = new TestRepo<Exercise>(new[] { new Exercise
                {
                    Equipment = "dumbbell",
                    Name = "Curls"
                } });
                A.CallTo(() => userContext.CanManageAllExercises()).Returns(true);
                var newExercise = new Exercise { Equipment = "barbell", Name = "New Exercise", FocusId = (byte)Models.Focus.Strength };
                var exerciseService = new ExerciseService { ExerciseRepository = exerciseRepository, UserContext = userContext };

                // Act
                var ex = Assert.ThrowsAsync<ArgumentException>(async () => await exerciseService.Add(newExercise));
                StringAssert.Contains("existing", ex.Message);
                StringAssert.Contains("equipment", ex.Message);
            }

            [Test]
            public void Should_Not_Add_Unauthorized()
            {

                // Arrange
                var userContext = A.Fake<IUserContext>();
                var exerciseRepository = new TestRepo<Exercise>(new[] { new Exercise
                {
                    Equipment = "dumbbell",
                    Name = "Curls"
                } });
                A.CallTo(() => userContext.CanManageAllExercises()).Returns(false);
                var newExercise = new Exercise { Equipment = "dumbbell", Name = "New Exercise", FocusId = (byte)Models.Focus.Strength };
                var exerciseService = new ExerciseService { ExerciseRepository = exerciseRepository, UserContext = userContext };

                // Act / Assert
                Assert.ThrowsAsync<SecurityException>(async () => await exerciseService.Add(newExercise));
            }
        }


        [TestFixture]
        public class When_Updating_Exercise
        {
            [Test]
            public async Task Should_Update()
            {
                var userContext = A.Fake<IUserContext>();
                var exercises = new[] { new Exercise { Id = 1, Equipment = "dumbbell", Name = "curls" }, new Exercise { Id = 2, Equipment = "dumbbell", Name = "lunges" } };
                var exerciseRepository = new TestRepo<Exercise>(exercises);
                A.CallTo(() => userContext.CanManageAllExercises()).Returns(true);
                var exerciseService = new ExerciseService { ExerciseRepository = exerciseRepository, UserContext = userContext };

                var newExercise = new Exercise { Id = 1, Equipment = "dumbbell", Name = "updated name" };
                await exerciseService.Update(newExercise);

                Assert.That(exerciseRepository.UpdatedIds.Single(), Is.EqualTo(1));

            }

            [Test]
            public void Should_Not_Update_Unauthorized()
            {
                var userContext = A.Fake<IUserContext>();
                var exercises = new[] { new Exercise { Id = 1, Equipment = "dumbbell", Name = "curls" }, new Exercise { Id = 2, Equipment = "dumbbell", Name = "lunges" } };
                var exerciseRepository = new TestRepo<Exercise>(exercises);
                A.CallTo(() => userContext.CanManageAllExercises()).Returns(false);
                var exerciseService = new ExerciseService { ExerciseRepository = exerciseRepository, UserContext = userContext };

                var newExercise = new Exercise { Id = 1, Equipment = "dumbbell", Name = "updated name" };

                Assert.ThrowsAsync<SecurityException>(() => exerciseService.Update(newExercise));
            }


            [Test]
            public void Should_Not_Update_New_Equipment()
            {
                var userContext = A.Fake<IUserContext>();
                var exercises = new[] { new Exercise { Id = 1, Equipment = "dumbbell", Name = "curls" }, new Exercise { Id = 2, Equipment = "dumbbell", Name = "lunges" } };
                var exerciseRepository = new TestRepo<Exercise>(exercises);
                A.CallTo(() => userContext.CanManageAllExercises()).Returns(true);
                var exerciseService = new ExerciseService { ExerciseRepository = exerciseRepository, UserContext = userContext };

                var newExercise = new Exercise { Id = 1, Equipment = "yoga ball", Name = "updated name" };

                var ex = Assert.ThrowsAsync<ArgumentException>(() => exerciseService.Update(newExercise));
                StringAssert.Contains("equipment", ex.Message);
            }
        }

        [TestFixture]
        public class When_Searching
        {
            ExerciseService ExerciseService = null!;

            [SetUp]
            public void Setup()
            {
                var exercises = new[]
                {
                    new Exercise { Id = 1, Name = "find by name", FocusId = (byte)Models.Focus.Cardio, Equipment = "xxx"},
                    new Exercise { Id = 2, Name = "find by name 2", FocusId = (byte) Models.Focus.Abs, Equipment = "xxx"},
                    new Exercise { Id = 3, Name = "xxx", FocusId = (byte)Models.Focus.Strength, Equipment = "find by equipment"},
                    new Exercise { Id = 4, Name = "xxx", FocusId = (byte)Models.Focus.Abs, Equipment = "find by equipment"}
                };
                var exerciseRepository = new TestRepo<Exercise>(exercises);
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.CanReadAllExercises()).Returns(true);
                ExerciseService = new ExerciseService { ExerciseRepository = exerciseRepository, UserContext = userContext };
            }

            [Test]
            public void Should_Reject_Unauthorized()
            {
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.CanReadAllExercises()).Returns(false);

                var exerciseService = new ExerciseService { UserContext = userContext };

                Assert.Throws<SecurityException>(() => exerciseService.Search(0, 0));
            }

            [Test]
            public void Should_Find_By_Name()
            {
                var results = ExerciseService.Search(10, 0, "find by name");
                Assert.That(results.Count, Is.EqualTo(2));
                Assert.That(results.First().Id, Is.EqualTo(1));
                Assert.That(results.Last().Id, Is.EqualTo(2));
            }

            [Test]
            public void Should_Find_By_Focus()
            {
                var results = ExerciseService.Search(10, 0, focus: "Abs");
                Assert.That(results.Count, Is.EqualTo(2));
                Assert.That(results.First().Id, Is.EqualTo(2));
                Assert.That(results.Last().Id, Is.EqualTo(4));

            }

            [Test]
            public void Should_Find_By_Equipment()
            {
                var results = ExerciseService.Search(10, 0, equipment: "find by equipment");
                Assert.That(results.Count, Is.EqualTo(2));
                Assert.That(results.First().Id, Is.EqualTo(3));
                Assert.That(results.Last().Id, Is.EqualTo(4));

            }
        }
    }
}