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
        }
    }
}