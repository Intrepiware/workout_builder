using Newtonsoft.Json;
using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Services.Impl
{
    public class WorkoutService : IWorkoutService
    {
        public IUserContext UserContext { init; protected get; } = null!;
        public IRepository<Workout> WorkoutRepository { init; protected get; } = null!;
        private const int WorkoutsPerUser = 100;

        public async Task<string?> Create(WorkoutGenerationResponseModel generatedWorkout)
        {
            var userId = UserContext.GetUserId();
            if (userId == null)
                return null;

            var counts = WorkoutRepository.GetAll().Where(x => x.UserId == userId)
                                .GroupBy(x => x.UserId)
                                .Select(x => new { Favorites = x.Where(y => y.IsFavorite).Count(), Total = x.Count() })
                                .SingleOrDefault();

            // If too many favorites; we can't delete anything. Don't save the routine. 
            if (counts != null && counts.Favorites >= WorkoutsPerUser)
                return null;

            if(counts != null && counts.Total >= WorkoutsPerUser)
            {
                // Trim the list so there are only 99 saved routines
                var take = counts.Total - (WorkoutsPerUser - 1);
                var deletes = WorkoutRepository.GetAll().Where(x => x.UserId == userId && !x.IsFavorite)
                                .OrderBy(x => x.CreateDate)
                                .Take(take)
                                .ToList();

                foreach (var delete in deletes)
                    await WorkoutRepository.Delete(delete);
            }


            var workout = new Workout
            {
                UserId = userId,
                Body = JsonConvert.SerializeObject(generatedWorkout),
                CreateDate = DateTime.UtcNow,
                PublicId = Guid.NewGuid().ToString("n")
            };
            await WorkoutRepository.Add(workout);
            return workout.PublicId;
        }

        public async Task<Workout> ToggleFavorite(string publicId)
        {
            var workout = WorkoutRepository.GetAll().SingleOrDefault(x => x.PublicId == publicId);
            if (workout == null)
                throw new ArgumentException("Cannot add favorite: unknown public id");

            if (UserContext.GetUserId() == null)
                throw new ArgumentException("Cannot add favorite: invalid user id");

            if(!UserContext.CanManageWorkoutFavorite(workout.UserId))
            {
                var newWorkout = new Workout
                {
                    Body = workout.Body,
                    CreateDate = DateTime.UtcNow,
                    IsFavorite = true,
                    PublicId = Guid.NewGuid().ToString("n"),
                    UserId = UserContext.GetUserId()
                };
                await WorkoutRepository.Add(newWorkout);
                return newWorkout;
            }
            else
            {
                workout.IsFavorite = !workout.IsFavorite;
                await WorkoutRepository.Update(workout);
                return workout;
            }
        }
    }
}
