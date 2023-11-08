using Autofac;
using WorkoutBuilder.Controllers;

namespace WorkoutBuilder.IOC
{
    public class AutofacRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<TestService>().As<ITestService>().InstancePerLifetimeScope();
            builder.RegisterType<HomeController>().PropertiesAutowired();
        }
    }
}
