using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Services.Impl
{
    public class ClaimsBasedUserContext : IUserContext
    {
        public IHttpContextAccessor HttpContextAccessor { init; protected get; } = null!;

        public string? GetEmailAddress() => User?.FindFirst(ClaimTypes.Email)?.Value;
        public long? GetUserId()
        {
            if (User != null)
            {
                if (long.TryParse(User.FindFirst(WorkoutBuilderClaimTypes.Id)?.Value, out var id))
                    return id;
            }

            return null;
        }

        protected ClaimsPrincipal? User => HttpContextAccessor.HttpContext.User;
    }
}
