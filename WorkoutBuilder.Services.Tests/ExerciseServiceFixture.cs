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
    }
}