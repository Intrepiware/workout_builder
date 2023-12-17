using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace WorkoutBuilder.Services.Impl
{
    public class UrlBuilder : IUrlBuilder
    {
        public IUrlHelperFactory UrlHelperFactory { init; protected get; }
        public IActionContextAccessor ActionContextAccessor { init; protected get; }

        public string Action(string action, string controller, object values) => Init().Action(action, controller, values);

        protected IUrlHelper Init() => UrlHelperFactory.GetUrlHelper(ActionContextAccessor.ActionContext);
    }
}
