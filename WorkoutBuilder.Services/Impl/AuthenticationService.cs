using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using WorkoutBuilder.Data;
using ClaimTypes = WorkoutBuilder.Services.Models.WorkoutBuilderClaimTypes;

namespace WorkoutBuilder.Services.Impl
{
    public class AuthenticationService : IAuthenticationService
    {
        public IPasswordHashingService PasswordHashingService { init; protected get; } = null!;
        public IRepository<User> UserRepository { init; protected get; } = null!;
        public IHttpContextAccessor HttpContextAccessor { init; protected get; } = null!;

        public async Task<bool> Login(string username, string password)
        {
            var user = UserRepository.GetAll().Where(x => x.EmailAddress == username).SingleOrDefault();
            if (user != null && PasswordHashingService.Verify(password, user.Password))
            {
                var claims = GetClaims(user);
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContextAccessor.HttpContext.SignInAsync(principal, 
                    new AuthenticationProperties { IsPersistent = true });
                return true;
            }

            return false;
        }

        private List<Claim> GetClaims(User user)
        {
            var output = new List<Claim>
            {
                new Claim(System.Security.Claims.ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.Id, user.Id.ToString()),
                new Claim($"{ClaimTypes.Users}/{user.Id}{ClaimTypes.Workouts}{ClaimTypes.Manage}", string.Empty),
                new Claim($"{ClaimTypes.Users}/{user.Id}{ClaimTypes.Workouts}{ClaimTypes.Favorites}{ClaimTypes.Manage}", string.Empty),
                new Claim($"{ClaimTypes.Exercises}{ClaimTypes.All}{ClaimTypes.Read}", string.Empty)
            };

            if (user.IsAdmin)
            {
                output.Add(new Claim($"{ClaimTypes.Users}/All{ClaimTypes.Workouts}{ClaimTypes.Manage}", string.Empty));
                output.Add(new Claim($"{ClaimTypes.Exercises}{ClaimTypes.All}{ClaimTypes.Manage}", string.Empty));
            }

            return output;
        }
    }
}