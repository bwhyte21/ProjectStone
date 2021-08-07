using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ProjectStone.Models;

namespace ProjectStone.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<RegisterModel> logger, IEmailSender emailSender, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string FullName { get; set; }
            public string PhoneNumber { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            // Create role if it does not exist. Use built in role condition check.
            // Will only be executed once for creation.
            if (!await _roleManager.RoleExistsAsync(WebConstants.AdminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(WebConstants.AdminRole));
                await _roleManager.CreateAsync(new IdentityRole(WebConstants.CustomerRole));
            }

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                // Scaffolded ver.
                //var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                // My ver.
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, PhoneNumber = Input.PhoneNumber, FullName = Input.FullName };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    // Check to see if an admin user has logged in.
                    if (User.IsInRole(WebConstants.AdminRole))
                    {
                        // First user will be an admin.
                        // Only admin can create admin at this point.
                        await _userManager.AddToRoleAsync(user, WebConstants.AdminRole);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, WebConstants.CustomerRole);
                    }
                    

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount) { return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl }); }
                    else
                    {
                        // This signs in user after [admin]account creation. We don't want this.
                        // If the new user is not in an admin role, then sign them in.
                        if (!User.IsInRole(WebConstants.AdminRole))
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }

                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors) { ModelState.AddModelError(string.Empty, error.Description); }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}