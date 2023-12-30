using Microsoft.AspNetCore.Mvc;
using WorkoutBuilder.Models;
using WorkoutBuilder.Services;

namespace WorkoutBuilder.ViewComponents
{
    public class TopMenuViewComponent : ViewComponent
    {
        public IUserContext UserContext { init; protected get; } = null!;
        public IUrlBuilder UrlBuilder { init; protected get; } = null!;

        public Task<IViewComponentResult> InvokeAsync()
        {
            var model = new TopMenuModel();
            model.Home = new MenuItemModel { DisplayName = "Home", Url = UrlBuilder.Action("Index", "Home", null) };
            model.Contact = new MenuItemModel { DisplayName = "Contact", Url = UrlBuilder.Action("Contact", "Home", null) };
            model.TimingCalc = new MenuItemModel { DisplayName = "Timing Calc", Url = UrlBuilder.Action("Index", "Timing", null) };
            if (UserContext.GetUserId() != null)
                model.Logout = new MenuItemModel { DisplayName = "Logout", Url = UrlBuilder.Action("Logout", "Users", null) };
            else
                model.Login = new MenuItemModel { DisplayName = "Login", Url = UrlBuilder.Action("Login", "Users", null) };

            IViewComponentResult result = View("TopMenu", model);
            return Task.FromResult(result);
        }
    }
}
