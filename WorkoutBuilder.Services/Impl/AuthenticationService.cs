using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Models;

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
                await HttpContextAccessor.HttpContext.SignInAsync(principal);
                return true;
            }

            return false;
        }

        private List<Claim> GetClaims(User user)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(WorkoutBuilderClaimTypes.Id, user.Id.ToString())
            };
        }
    }
}
