using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Impl;
using WorkoutBuilder.Services.Models;
using WorkoutBuilder.Services.Tests.TestUtilities;

namespace WorkoutBuilder.Services.Tests
{
    public class WorkoutServiceFixture
    {
        [TestFixture]
        public class When_Creating_Workout
        {
            [Test]
            public async Task Should_Save()
            {
                var workoutRepo = new TestRepo<Workout>();
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.GetUserId()).Returns(1L);

                var workoutService = new WorkoutService { UserContext = userContext, WorkoutRepository = workoutRepo };

                var id = await workoutService.Create(new WorkoutGenerationResponseModel());

                Assert.IsNotNull(id);
                var created = workoutRepo.AddedItems.Single();
                Assert.That(created.PublicId, Is.EqualTo(id));
                Assert.That(created.UserId, Is.EqualTo(1));
                Assert.That(created.CreateDate, Is.GreaterThan(DateTime.UtcNow.AddMinutes(-1)));
            }

            [Test]
            public async Task Should_Not_Save_Anonymous()
            {
                var workoutRepo = new TestRepo<Workout>();
                var userContext = A.Fake<IUserContext>();
                var workoutService = new WorkoutService { UserContext = userContext, WorkoutRepository = workoutRepo };

                var id = await workoutService.Create(new WorkoutGenerationResponseModel());

                Assert.IsNull(id);
                Assert.That(workoutRepo.AddedItems.Count, Is.EqualTo(0));
            }

            [TestFixture]
            public class When_Too_Many_Workouts_Saved
            {
                [Test]
                public async Task Should_Delete_Saved_Workouts()
                {
                    var workouts = Enumerable.Range(1, 101) // 101 existing records, so *2* should be deleted
                                    .Select(x => new Workout { Id = x, UserId = 1 });

                    var workoutRepo = new TestRepo<Workout>(workouts);
                    var userContext = A.Fake<IUserContext>();
                    A.CallTo(() => userContext.GetUserId()).Returns(1L);

                    var workoutService = new WorkoutService { UserContext = userContext, WorkoutRepository = workoutRepo };

                    var id = await workoutService.Create(new WorkoutGenerationResponseModel());
                    Assert.IsNotNull(id);
                    Assert.That(workoutRepo.AddedItems.Count, Is.EqualTo(1));
                    Assert.That(workoutRepo.DeletedItems.Count, Is.EqualTo(2));
                }

                [Test]
                public async Task Should_Not_Create_When_All_Favorites()
                {
                    var workouts = Enumerable.Range(1, 100)
                                    .Select(x => new Workout { Id = x, UserId = 1, IsFavorite = true });

                    var workoutRepo = new TestRepo<Workout>(workouts);
                    var userContext = A.Fake<IUserContext>();
                    A.CallTo(() => userContext.GetUserId()).Returns(1L);

                    var workoutService = new WorkoutService { UserContext = userContext, WorkoutRepository = workoutRepo };

                    var id = await workoutService.Create(new WorkoutGenerationResponseModel());
                    Assert.IsNull(id);
                    Assert.That(workoutRepo.AddedItems.Count, Is.EqualTo(0));
                    Assert.That(workoutRepo.DeletedItems.Count, Is.EqualTo(0));

                }

                [Test]
                public async Task Should_Not_Delete_Favorites()
                {
                    var now = DateTime.UtcNow;
                    // Generate a list of 100 workouts, with the first two as "favorites"
                    var workouts = new List<Workout>
                    {
                        new Workout { Id = 1, UserId = 1, IsFavorite = true, CreateDate = now.AddHours(-100)},
                        new Workout { Id = 2, UserId = 1, IsFavorite = true, CreateDate = now.AddHours(-99)},
                        new Workout { Id = 3, UserId = 1, IsFavorite = false, CreateDate = now.AddHours(-98)} // This one should be deleted
                    };
                    workouts.AddRange(Enumerable.Range(4, 97).Select(x => new Workout { Id = x, UserId = 1, CreateDate = now.AddHours(x - 100) }));
                    var workoutRepo = new TestRepo<Workout>(workouts);
                    var userContext = A.Fake<IUserContext>();
                    A.CallTo(() => userContext.GetUserId()).Returns(1L);

                    var workoutService = new WorkoutService { UserContext = userContext, WorkoutRepository = workoutRepo };

                    var id = await workoutService.Create(new WorkoutGenerationResponseModel());
                    Assert.IsNotNull(id);
                    Assert.That(workoutRepo.AddedItems.Count, Is.EqualTo(1));
                    Assert.That(workoutRepo.DeletedIds.Single(), Is.EqualTo(3));

                }

                [Test]
                public async Task Should_Not_Delete_Wrong_User()
                {
                    var workouts = Enumerable.Range(1, 101)
                                    .Select(x => new Workout { Id = x, UserId = 2 });

                    workouts = workouts.Append(new Workout { Id = 102, UserId = 1 });

                    var workoutRepo = new TestRepo<Workout>(workouts);
                    var userContext = A.Fake<IUserContext>();
                    A.CallTo(() => userContext.GetUserId()).Returns(1L);

                    var workoutService = new WorkoutService { UserContext = userContext, WorkoutRepository = workoutRepo };

                    var id = await workoutService.Create(new WorkoutGenerationResponseModel());
                    Assert.IsNotNull(id);
                    Assert.That(workoutRepo.AddedItems.Count, Is.EqualTo(1));
                    Assert.That(workoutRepo.DeletedItems.Count, Is.EqualTo(0));
                }

                [Test]
                public async Task Should_Delete_Oldest()
                {
                    var now = DateTime.UtcNow;
                    var workouts = Enumerable.Range(1, 99).Select(x => new Workout { Id = x, UserId = 1, CreateDate = now }).ToList();
                    workouts.Add(new Workout { Id = 101, UserId = 1, CreateDate = now.AddDays(-100) }); // This one should be deleted
                    var workoutRepo = new TestRepo<Workout>(workouts);
                    var userContext = A.Fake<IUserContext>();
                    A.CallTo(() => userContext.GetUserId()).Returns(1L);

                    var workoutService = new WorkoutService { UserContext = userContext, WorkoutRepository = workoutRepo };

                    var id = await workoutService.Create(new WorkoutGenerationResponseModel());
                    Assert.IsNotNull(id);
                    Assert.That(workoutRepo.AddedItems.Count, Is.EqualTo(1));
                    Assert.That(workoutRepo.DeletedIds.Single(), Is.EqualTo(101));
                }
            }
        }

        [TestFixture]
        public class When_Toggling_Favorite
        {
            [Test]
            public async Task Should_Favorite()
            {
                var workouts = new List<Workout> { new Workout { Id = 1, PublicId = "workout1", UserId = 1 } };
                var workoutRepo = new TestRepo<Workout>(workouts);
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.GetUserId()).Returns(1L);
                A.CallTo(() => userContext.CanManageWorkoutFavorite(1L)).Returns(true);
                var workoutService = new WorkoutService { UserContext = userContext, WorkoutRepository = workoutRepo };

                var result = await workoutService.ToggleFavorite("workout1");

                Assert.That(result.Id, Is.EqualTo(1L));
                Assert.That(workoutRepo.UpdatedItems.Single().Id, Is.EqualTo(1L));
                Assert.IsTrue(workoutRepo.UpdatedItems.Single().IsFavorite);
            }

            [Test]
            public async Task Should_Unfavorite()
            {
                var workouts = new List<Workout> { new Workout { Id = 1, PublicId = "workout1", UserId = 1, IsFavorite = true } };
                var workoutRepo = new TestRepo<Workout>(workouts);
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.GetUserId()).Returns(1L);
                A.CallTo(() => userContext.CanManageWorkoutFavorite(1L)).Returns(true);
                var workoutService = new WorkoutService { UserContext = userContext, WorkoutRepository = workoutRepo };

                var result = await workoutService.ToggleFavorite("workout1");

                Assert.That(result.Id, Is.EqualTo(1L));
                Assert.That(workoutRepo.UpdatedItems.Single().Id, Is.EqualTo(1L));
                Assert.IsFalse(workoutRepo.UpdatedItems.Single().IsFavorite);
            }

            [TestFixture]
            public class When_UserId_Mismatch
            {
                [Test]
                public async Task Should_Clone()
                {
                    var workouts = new List<Workout> { new Workout { Id = 1, PublicId = "workout1", UserId = 2, IsFavorite = true } };
                    var workoutRepo = new TestRepo<Workout>(workouts);
                    var userContext = A.Fake<IUserContext>();
                    A.CallTo(() => userContext.GetUserId()).Returns(1L);
                    A.CallTo(() => userContext.CanManageWorkoutFavorite(1L)).Returns(true);
                    var workoutService = new WorkoutService { UserContext = userContext, WorkoutRepository = workoutRepo };

                    var result = await workoutService.ToggleFavorite("workout1");

                    Assert.That(result.PublicId, Is.Not.EqualTo("workout1"));
                    Assert.That(result.PublicId, Is.EqualTo(workoutRepo.AddedItems.Single().PublicId));
                    Assert.IsTrue(workoutRepo.AddedItems.Single().IsFavorite);
                }
            }

            [TestFixture]
            public class When_Unknown_Workout
            {
                [Test]
                public void Should_Throw()
                {
                    var workouts = new List<Workout> { new Workout { Id = 1, PublicId = "workout1", UserId = 1 } };
                    var workoutRepo = new TestRepo<Workout>(workouts);
                    var userContext = A.Fake<IUserContext>();
                    A.CallTo(() => userContext.GetUserId()).Returns(1L);
                    A.CallTo(() => userContext.CanManageWorkoutFavorite(1L)).Returns(true);
                    var workoutService = new WorkoutService { UserContext = userContext, WorkoutRepository = workoutRepo };

                    Assert.ThrowsAsync<ArgumentException>(() => workoutService.ToggleFavorite("bogusId"));

                    Assert.That(workoutRepo.UpdatedItems.Count, Is.EqualTo(0));
                }
            }

            [TestFixture]
            public class When_Run_Anonymously
            {
                [Test]
                public void Should_Throw()
                {
                    var workouts = new List<Workout> { new Workout { Id = 1, PublicId = "workout1", UserId = 1, IsFavorite = true } };
                    var workoutRepo = new TestRepo<Workout>(workouts);
                    var userContext = A.Fake<IUserContext>();
                    var workoutService = new WorkoutService { UserContext = userContext, WorkoutRepository = workoutRepo };

                    Assert.ThrowsAsync<ArgumentException>(() => workoutService.ToggleFavorite("workout1"));
                    Assert.That(workoutRepo.UpdatedItems.Count, Is.EqualTo(0));
                }
            }
        }
    }
}
