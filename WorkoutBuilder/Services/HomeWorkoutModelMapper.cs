using Newtonsoft.Json;
using WorkoutBuilder.Data;
using WorkoutBuilder.Models;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Services
{
    public interface IHomeWorkoutModelMapper
    {
        Task<HomeWorkoutModel> Map(WorkoutGenerationResponseModel data);
        HomeWorkoutModel Map(Workout workout, string publicId);
    }
    public class HomeWorkoutModelMapper : IHomeWorkoutModelMapper
    {
        public IWorkoutService WorkoutService { init; protected get; } = null!;
        public IUserContext UserContext { init; protected get; } = null!;
        public async Task<HomeWorkoutModel> Map(WorkoutGenerationResponseModel data)
        {
            var publicId = await WorkoutService.Create(data);

            var model = new HomeWorkoutModel
            {
                PublicId = publicId,
                Workout = data
            };

            return model;
        }

        public HomeWorkoutModel Map(Workout workout, string publicId)
        {
            var output = new HomeWorkoutModel
            {
                Workout = JsonConvert.DeserializeObject<WorkoutGenerationResponseModel>(workout.Body),
                PublicId = publicId,
                IsFavorite = UserContext.GetUserId() == workout.UserId && workout.IsFavorite
            };

            return output;
        }
    }
}
