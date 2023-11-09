using Autofac;
using Microsoft.EntityFrameworkCore;
using WorkoutBuilder.Controllers;
using WorkoutBuilder.Data;
using WorkoutBuilder.Services;
using WorkoutBuilder.Services.Impl;

namespace WorkoutBuilder.IOC
{
    public class AutofacRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Services
            builder.RegisterType<RandomizeService>().As<IRandomize>().InstancePerLifetimeScope();

            // Repositories
            builder.RegisterType<WorkoutBuilderContext>().As<DbContext>().InstancePerLifetimeScope();
            RegisterRepository<Exercise>(builder);
            RegisterRepository<Focus>(builder);
            RegisterRepository<Timing>(builder);
            
            // Controllers
            builder.RegisterType<HomeController>().PropertiesAutowired();
        }

        protected void RegisterRepository<TModel>(ContainerBuilder builder) where TModel : class
        {
            builder.Register<IRepository<TModel>>(c => new Repository<TModel>(c.Resolve<DbContext>())).InstancePerLifetimeScope();
        }
    }
}
