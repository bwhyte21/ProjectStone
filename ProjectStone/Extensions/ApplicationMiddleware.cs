using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ProjectStone_DataAccess.Initializer;

namespace ProjectStone.Extensions
{
  public static class ApplicationMiddleware
  {
      public static IApplicationBuilder AddApplicationMiddleware(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config, IDbInitializer dbInitializer)
      {
          #region SyncFusion License Config

          // Register Syncfusion License.
          Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(config["SyncFusion:LicenseKey"]);

          #endregion

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

          #region DBInitializer Pipeline Config

          // Invoke the initializer located in DataAccess.
          dbInitializer.Initialize();

          #endregion

          #region Session Pipeline Config

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

          return app;
      }
  }
}
