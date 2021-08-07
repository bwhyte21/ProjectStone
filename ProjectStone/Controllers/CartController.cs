using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using ProjectStone.Data;
using ProjectStone.Models;
using ProjectStone.Models.ViewModels;
using ProjectStone.Utility;

namespace ProjectStone.Controllers
{
    // Protect the cart. User must be logged in to see the cart.
    [Authorize] // Can be placed at Controller level (here) or individual access level (e.g.; Index)
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        
        // Once it's bound in the post, it does not need to be explicitly defined in the action method's parameter. It will be available by default in the summary post.
        [BindProperty]
        public ProductUserViewModel ProductUserVm { get; set; }

        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var shoppingCartList = new List<ShoppingCart>();

            // Check for session.
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) is not null 
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
            {
                // Session exists. Set the shopping cart list to the existing one in the session.
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            // Using projections to get all products in the shopping cart into a form.
            var prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            
            // This acts as in IN clause in SQL. 
            // Retrieve all products WHERE Id mataches any Id inside the ProdInCart list.
            var prodList = _db.Product.Where(u => prodInCart.Contains(u.Id));

            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            
            return RedirectToAction(nameof(Summary));
        }
        
        // Cart Summary
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier); // Gets populated if user has logged in.
            //var userId = User.FindFirstValue(ClaimTypes.Name); // Retrieves user's Id

            // Get session, load list from session.
            var shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) is not null 
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            var prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            var prodList = _db.Product.Where(u => prodInCart.Contains(u.Id));


            // Retrieve the user details based on the user's ID.
            ProductUserVm = new ProductUserViewModel
            {
                // claim.Value should have the Id of the logged in user.
                ApplicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value),
                ProductList = prodList.ToList()
            };

            return View(ProductUserVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserViewModel productUserViewModel)
        {
            // Send an email after inquiry submission, then show confirmation page.
            var templatePath = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar + "templates" + Path.DirectorySeparatorChar + "Inquiry.html";

            const string emailSubject = "New Inquiry";
            var htmlBody = "";
            using (var streamReader = System.IO.File.OpenText(templatePath))
            {
                // Stream the html template into the html body.
                htmlBody = await streamReader.ReadToEndAsync();
            }

            // Products for {3} in the template.
            var productListSb = new StringBuilder();
            foreach (var prod in productUserViewModel.ProductList)
            {
                productListSb.Append($" - Name: {prod.Name} <span style='font-size:14px;'>(ID: {prod.Id})</span><br/>");
            }

            var messageBody = string.Format(htmlBody, 
                productUserViewModel.ApplicationUser.FullName, productUserViewModel.ApplicationUser.Email, 
                productUserViewModel.ApplicationUser.PhoneNumber, productListSb);

            // Send inquiry email to admin.
            await _emailSender.SendEmailAsync(WebConstants.AdminEmail, emailSubject, messageBody);

            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation()
        {
            // Clear user session after inquiry submission.
            HttpContext.Session.Clear();
      
            return View();
        }

        public IActionResult Remove(int id)
        {
            var shoppingCartList = new List<ShoppingCart>();

            // Check for session.
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) is not null 
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
            {
                // Session exists. Set the shopping cart list to the existing one in the session.
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            // Find the desired object and remove it from the list.
            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.ProductId == id));

            // Update the session with the updated cart list.
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);

            // Then redirect to index.
            return RedirectToAction(nameof(Index));
        }
    }
}