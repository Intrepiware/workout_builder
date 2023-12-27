using WorkoutBuilder.Models;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Services
{
    public interface IHomeWorkoutModelMapper
    {
        Task<HomeWorkoutModel> Map(WorkoutGenerationResponseModel data);
    }
    public class HomeWorkoutModelMapper : IHomeWorkoutModelMapper
    {
        public IWorkoutService WorkoutService { init; protected get; } = null!;
        public IUserContext UserContext { init; protected get; } = null!;
        public async Task<HomeWorkoutModel> Map(WorkoutGenerationResponseModel data)
        {
            var publicId = await WorkoutService.Create(data);

            var model = new HomeWorkoutModel { 
                PublicId = publicId, 
                Workout = data,
                Permissions = new List<string>()
            };

            if (UserContext.GetUserId().HasValue)
                model.Permissions.Add("favorite");

            return model;
        }
    }
}
