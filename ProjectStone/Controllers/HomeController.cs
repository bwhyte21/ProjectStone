using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Models;
using ProjectStone_Models.ViewModels;
using ProjectStone_Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ProjectStone.Controllers
{
  public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepo, ICategoryRepository categoryRepo)
        {
            _logger = logger;
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        public IActionResult Index()
        {
            var homeViewModel = new HomeViewModel
            {
                Products = _productRepo.GetAll(includeProperties:"Category,SubCategory"),
                Categories = _categoryRepo.GetAll()
            };

            return View(homeViewModel);
        }

        public IActionResult Details(int id)
        {
            // Retrieve session
            var shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) is not null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            var detailsViewModel = new DetailsViewModel
            {
                Product = _productRepo.FirstOrDefault(u => u.Id == id, "Category,SubCategory"),
                IsInCart = false
            };

            foreach (var item in shoppingCartList)
            {
                if (item.ProductId == id) { detailsViewModel.IsInCart = true; }
            }

            return View(detailsViewModel);
        }

        [HttpPost]
        [ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            var shoppingCartList = new List<ShoppingCart>();

            // If it has no values in it, add to cart.
            // If it has values, retrieve it, append product to cart.
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) is not null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            // Add item to shopping cart.
            shoppingCartList.Add(new ShoppingCart { ProductId = id });

            // Set Session.
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);

            TempData[WebConstants.Success] = "Item added to cart.";
            
            // Using nameof() instead of magic strings to prevent url mix-ups, e.g; "Index"
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            var shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) is not null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            // Using LINQ, get the item to remove.
            var itemToRemove = shoppingCartList.SingleOrDefault(r => r.ProductId == id);

            if (itemToRemove is not null)
            {
                shoppingCartList.Remove(itemToRemove);
            }
            
            // Set the session again, this time, with the new list.
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);
            TempData[WebConstants.Success] = "Item removed from cart.";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}