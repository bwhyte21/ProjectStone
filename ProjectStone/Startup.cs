using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ProjectStone.Data;
using ProjectStone.Utility;

namespace ProjectStone
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region DBContext Services

            // Adding a DbContext Service.
            // This uses the connection string that was created in appsettings.json file.
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            #endregion

            #region Identity Services

            // Add Identity service. Install NuGet pkg Microsoft.AspNetCore.Identity.UI
            // .AddEntityFrameworkStores creates the identity tables in our DB for us.
            // Add identityRole to config Role Manager while using "AddIdentity".
            //services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<ApplicationDbContext>(); // We use the one below since we've added roles.

            // .AddDefaultTokenProviders for when "Forgot Password" is invoked. [Needed along with Identity Role]
            // .AddDefaultUI is for Identity pages and more. [Needed along with Identity Role]
            services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddDefaultUI().AddEntityFrameworkStores<ApplicationDbContext>();

            #endregion

            #region Email Services

            // Email service registration.
            services.AddTransient<IEmailSender, EmailSender>();

            #endregion

            #region Session Services

            // Add Sessions to this .Net Core project.
            services.AddHttpContextAccessor();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            #endregion

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Use files in wwwRoot folder.
            app.UseStaticFiles();

            app.UseRouting();

            #region Identity Pipeline Config

            // Now that we're using Identity, we must add UseAuthentication BEFORE UseAuthorization.
            // Note: After migrating Identity into the DB, right-click the Project, go to "Add", click "Add New Scaffolded Item", then select Identity to choose
            // then add in the desired Identity Pages (Logins, ForgotPassword, etc)
            app.UseAuthentication();

            #endregion

            app.UseAuthorization();

            #region Session Pipeline Cofig

            // Extra Session service extensions are to be located in a "Utility" folder.
            app.UseSession();

            #endregion

            app.UseEndpoints(endpoints =>
            {
                // Routing for Razor Pages. Needed after Scaffolding Identity Razor Pages.
                endpoints.MapRazorPages();
                // Routing for MVC.
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}