using Autofac;
using Autofac.Extensions.DependencyInjection;
using BotDetect.Web;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using WorkoutBuilder.Data;
using WorkoutBuilder.IOC;
using WorkoutBuilder.Middleware;

namespace WorkoutBuilder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<WorkoutBuilderContext>(opt => opt.UseLazyLoadingProxies()
            .UseSqlServer(builder.Configuration.GetConnectionString("WorkoutBuilderConnection")));

            // Add services to the container.
            IConfiguration configuration = builder.Configuration;
            builder.Services.AddMvc().AddControllersAsServices().AddViewComponentsAsServices();

            // Register AutoFac
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new AutofacRegistrationModule(configuration)));

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.IsEssential = true;
            });


            builder.Services.AddAuthentication("CookieAuth")
                    .AddCookie("CookieAuth", config =>
                    {
                        config.Cookie.Name = "WorkoutBuild";
                        config.LoginPath = "/Users/Login";
                    });

            // This setting allows the CAPTCHA to generate images
            builder.Services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);
            builder.Services.Configure<IISServerOptions>(options => options.AllowSynchronousIO = true);

            // RegisterApplication Insights
            var aiOptions = new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions();
            builder.Services.AddApplicationInsightsTelemetry(aiOptions);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseMiddleware<EmailExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();
            app.UseCaptcha(configuration);

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Automatically run migrations
            using (var scope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<WorkoutBuilderContext>().Database.Migrate();
            }
            app.Run();
        }
    }
}