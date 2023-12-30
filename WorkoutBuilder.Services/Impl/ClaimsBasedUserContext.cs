using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using WorkoutBuilder.Services.Models;
using ClaimTypes = WorkoutBuilder.Services.Models.WorkoutBuilderClaimTypes;

namespace WorkoutBuilder.Services.Impl
{
    public class ClaimsBasedUserContext : IUserContext
    {
        public IHttpContextAccessor HttpContextAccessor { init; protected get; } = null!;

        public string? GetEmailAddress() => User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        public long? GetUserId()
        {
            if (User != null)
            {
                if (long.TryParse(User.FindFirst(ClaimTypes.Id)?.Value, out var id))
                    return id;
            }

            return null;
        }

        public bool CanManageWorkout(long? userId)
        {
            if (User == null)
                return false;

            if (User.HasClaim(x => x.Type == $"{ClaimTypes.Users}/All{ClaimTypes.Workouts}{ClaimTypes.Manage}"))
                return true;

            if (userId.HasValue)
            {
                if (User.HasClaim(x => x.Type == $"{ClaimTypes.Users}/{userId.Value}{ClaimTypes.Workouts}{ClaimTypes.Manage}"))
                    return true;
            }

            return false;

        }
        public bool CanManageWorkoutFavorite(long? userId)
        {
            if (User == null)
                return false;

            if (userId.HasValue)
            {
                if (User.HasClaim(x => x.Type == $"{ClaimTypes.Users}/{userId.Value}{ClaimTypes.Workouts}{ClaimTypes.Favorites}{ClaimTypes.Manage}"))
                    return true;
            }

            return false;
        }

        protected ClaimsPrincipal? User => HttpContextAccessor.HttpContext.User;
    }
}
