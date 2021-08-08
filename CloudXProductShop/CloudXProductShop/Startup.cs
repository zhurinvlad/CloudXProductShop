using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using CloudXProductShop.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CloudXProductShop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //AWSSDKHandler.RegisterXRayForAllServices(); // All AWS SDK requests will be traced
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var secretConnctionString = AWSSecretManager.GetRDSSecret();
            services.AddDbContext<ProductShopContext>(options =>
            {
                options
                    .UseMySql(secretConnctionString, new MySqlServerVersion(new Version(8, 0, 25)))
                    .EnableSensitiveDataLogging() // <-- These two calls are optional but help
                    .EnableDetailedErrors(); // <-- with debugging (remove for production).
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                //app.UseXRay("CloudXProductShop"); // name of the app

            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ProductShopContext>())
                {
                    context.Database.Migrate();
                }
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
