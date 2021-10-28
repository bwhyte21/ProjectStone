using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectStone_DataAccess.Data;
using ProjectStone_DataAccess.Initializer;
using ProjectStone_DataAccess.Repository;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Utility;
using ProjectStone_Utility.BrainTree;

namespace ProjectStone.Extensions
{
  public static class ApplicationServices
  {
      public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
      {
          
          #region DBContext Services

            // Adding a DbContext Service.
            // This uses the connection string that was created in appsettings.json file. (Dependency Injection)
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

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
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
            });

            #endregion

            #region Braintree Services for 3rd Party Payments using Dependency Injection

            // Configure BrainTree settings based on appsettings.json file. This method works the same as how MailJet was setup, but this way is cleaner.
            //  Everything should be registered ONCE, here, in the container, Startup.cs
            services.Configure<BrainTreeSettings>(config.GetSection("BrainTree"));
            services.AddSingleton<IBrainTreeGate, BrainTreeGate>();

            #endregion

            #region Repository Services for Dependency Injection

            // Register Repository containers here.
            // We will use AddScoped for databases to be used for the scope's lifetime. Meaning it will stay for one request. 
            // If we use it multiple times, it will still use the same object.
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IInquiryHeaderRepository, InquiryHeaderRepository>();
            services.AddScoped<IInquiryDetailRepository, InquiryDetailRepository>();
            services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();

            // This service will be for the DbInitializer with the purpose of Seeding the DB in a new environment for deployment.
            // This will also be added to the pipeline config below.
            services.AddScoped<IDbInitializer, DbInitializer>();

            #endregion

            #region Facebook Service Authentication

            // Register Facebook SSOAuth service to project.
            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = config["FaceBook:AppId"];
                options.AppSecret = config["FaceBook:AppSecret"];
            });

            #endregion

          return services;
      }
  }
}
