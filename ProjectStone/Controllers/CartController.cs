using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Models;
using ProjectStone_Models.ViewModels;
using ProjectStone_Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectStone.Controllers
{
  // Protect the cart. User must be logged in to see the cart.
  [Authorize] // Can be placed at Controller level (here) or individual access level (e.g.; Index)
    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IApplicationUserRepository _userRepo;
        private readonly IProductRepository _productRepo;
        private readonly IInquiryHeaderRepository _inqHeaderRepo;
        private readonly IInquiryDetailRepository _inqDetailRepo;

        // Once it's bound in the post, it does not need to be explicitly defined in the action method's parameter. It will be available by default in the summary post.
        [BindProperty]
        public ProductUserViewModel ProductUserVm { get; set; }

        public CartController(IWebHostEnvironment webHostEnvironment, IEmailSender emailSender, IApplicationUserRepository userRepo, IProductRepository productRepo,
            IInquiryHeaderRepository inqHeaderRepo, IInquiryDetailRepository inqDetailRepo)
        {
            // Immutable objects.
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _userRepo = userRepo;
            _productRepo = productRepo;
            _inqHeaderRepo = inqHeaderRepo;
            _inqDetailRepo = inqDetailRepo;
        }

        public IActionResult Index()
        {
            var shoppingCartList = new List<ShoppingCart>();

            // Check for session.
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) is not null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
            {
                // Session exists. Set the shopping cart list to the existing one in the session.
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            // Using projections to get all products in the shopping cart into a form.
            var prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();

            // This acts as in IN clause in SQL. 
            // Retrieve all products WHERE Id matches any Id inside the ProdInCart list.
            var prodListTemp = _productRepo.GetAll(u => prodInCart.Contains(u.Id)).ToList(); // New: Set this IEnumerable .ToList() to prevent possible multiple enumerations in the foreach below.
            IList<Product> prodList = new List<Product>();

            // We need to now populate the SqFt in the product list based on the session in the cart.
            foreach (var cartObj in shoppingCartList)
            {
                var prodTemp = prodListTemp.FirstOrDefault(u => u.Id == cartObj.ProductId);

                if (prodTemp == null) continue;
                
                prodTemp.TempSqFt = cartObj.SqFt;
                prodList.Add(prodTemp);
            }

            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> productList)
        {
            // In case a user decides to change the value THEN click continue, rather than Update.
            var shoppingCartList = new List<ShoppingCart>();

            // Iterate through all the objects to set the SqFt value.
            foreach (var product in productList)
            {
                shoppingCartList.Add(new ShoppingCart
                {
                    ProductId = product.Id,
                    SqFt = product.TempSqFt
                });
            }

            // Update current session with updated shopping cart.
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);
            
            // Navigate to Summary.
            return RedirectToAction(nameof(Summary));
        }

        // Cart Summary
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            // If claimsIdentity is null for some reason, bounce.
            if (claimsIdentity is null) return View(ProductUserVm);

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier); // Gets populated if user has logged in.
            //var userId = User.FindFirstValue(ClaimTypes.Name); // Retrieves user's Id

            // Get session, load list from session.
            var shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) is not null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            var prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            var prodList = _productRepo.GetAll(u => prodInCart.Contains(u.Id));

            // Retrieve the user details based on the user's ID.
            ProductUserVm = new ProductUserViewModel
            {
                // claim.Value should have the Id of the logged in user.
                ApplicationUser = _userRepo.FirstOrDefault(u => u.Id == claim.Value),
                ProductList = prodList.ToList()
            };

            return View(ProductUserVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserViewModel productUserViewModel)
        {
            // Capture App User Id.
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            if (claimsIdentity is not null)
            {
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                // Send an email after inquiry submission, then show confirmation page.
                var templatePath = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar + "templates" + Path.DirectorySeparatorChar + "Inquiry.html";

                const string emailSubject = "New Inquiry";
                string htmlBody;
                using (var streamReader = System.IO.File.OpenText(templatePath))
                {
                    // Stream the html template into the html body.
                    htmlBody = await streamReader.ReadToEndAsync();
                }

                // Products for {3} in the template.
                var productListSb = new StringBuilder();
                foreach (var prod in productUserViewModel.ProductList) { productListSb.Append($" - Name: {prod.Name} <span style='font-size:14px;'>(ID: {prod.Id})</span><br/>"); }

                var messageBody = string.Format(htmlBody, productUserViewModel.ApplicationUser.FullName, productUserViewModel.ApplicationUser.Email, productUserViewModel.ApplicationUser.PhoneNumber,
                    productListSb);

                // Send inquiry email to admin.
                await _emailSender.SendEmailAsync(WebConstants.AdminEmail, emailSubject, messageBody);

                // Add Inquiry Header and Detail to DB.
                var inquiryHeader = new InquiryHeader
                {
                    ApplicationUserId = claim?.Value,
                    FullName = productUserViewModel.ApplicationUser.FullName,
                    Email = productUserViewModel.ApplicationUser.Email,
                    PhoneNumber = productUserViewModel.ApplicationUser.PhoneNumber,
                    InquiryDate = DateTime.Now
                };

                // Get Id of header to populate in detail. Add record to db.
                _inqHeaderRepo.Add(inquiryHeader);
                // Save to DB.
                _inqHeaderRepo.Save();

                // Create records per item in cart.
                foreach (var product in productUserViewModel.ProductList)
                {
                    // For each product, there is to be an inquiry detail.
                    var inquiryDetail = new InquiryDetail
                    {
                        InquiryHeaderId = inquiryHeader.Id,
                        ProductId = product.Id
                    };

                    // Add detail to DB.
                    _inqDetailRepo.Add(inquiryDetail);
                }
            }

            // Save to DB.
            _inqDetailRepo.Save();
            TempData[WebConstants.Success] = "Inquiry submitted successfully!";


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
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) is not null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> productList)
        {
            var shoppingCartList = new List<ShoppingCart>();

            // Iterate through all the objects to set the SqFt value.
            foreach (var product in productList)
            {
                shoppingCartList.Add(new ShoppingCart
                {
                    ProductId = product.Id,
                    SqFt = product.TempSqFt
                });
            }

            // Update current session with updated shopping cart.
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }
    }
}