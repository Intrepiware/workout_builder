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
        public IConfiguration Configuration { get; }
        
        public AutofacRegistrationModule(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        protected override void Load(ContainerBuilder builder)
        {
            // Services
            builder.RegisterType<RandomizeService>().As<IRandomize>().InstancePerLifetimeScope();
            builder.RegisterType<GeneralWorkoutGenerator>().As<IWorkoutGenerator>().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterType<WorkoutGeneratorFactory>().As<IWorkoutGeneratorFactory>().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();

            if (Configuration["InjectionMode"] == "development")
            {
                builder.RegisterType<FakeEmailService>().As<IEmailService>().PropertiesAutowired().InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterType<SendgridEmailService>().As<IEmailService>().PropertiesAutowired().InstancePerLifetimeScope();
            }


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
