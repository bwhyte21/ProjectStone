using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectStone_DataAccess.Data;
using ProjectStone_Models;
using ProjectStone_Utility;
using System;
using System.Linq;

namespace ProjectStone_DataAccess.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        #region Dependency Injection

        // Using Dependency Injection, we will check for any pending Migrations using AppDBContext.
        // Then we need to create Admin/Client roles via UserManager and Role Manager.
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #endregion

        public void Initialize()
        {
            try
            {
                // Check for any pending DbMigrations. If any, push migrations to database.
                if (_db.Database.GetPendingMigrations().Any()) { _db.Database.Migrate(); }
            }
            catch (Exception ex) { throw; }

            #region Role Creation

            // Create role if it does not exist. Use built in role condition check. Use GetAwaiter since this method is not
            //  Async, to wait for this statement to be fetched, then return the result.
            if (!_roleManager.RoleExistsAsync(WebConstants.AdminRole).GetAwaiter().GetResult())
            {
                // Make sure these statements are executed, then get the results.
                _roleManager.CreateAsync(new IdentityRole(WebConstants.AdminRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(WebConstants.CustomerRole)).GetAwaiter().GetResult();
            }
            else
            {
                return;
            }

            #endregion

            #region User Creation

            // Create user and assign all properties we want.
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com", // System Admin 
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                FullName = "Admin Tester",
                PhoneNumber = "4431112222"
            }, "Pa$$123!").GetAwaiter().GetResult();

            // Now assign THIS user the role of ADMIN.
            var user = _db.ApplicationUser.FirstOrDefault(u => u.Email == "admin@gmail.com");
            _userManager.AddToRoleAsync(user, WebConstants.AdminRole).GetAwaiter().GetResult();

            #endregion
        }
    }
}