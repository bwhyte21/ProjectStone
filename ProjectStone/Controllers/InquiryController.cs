using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Models;
using ProjectStone_Models.ViewModels;
using ProjectStone_Utility;
using System.Collections.Generic;

namespace ProjectStone.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderRepository _inqHeaderRepo;
        private readonly IInquiryDetailRepository _inqDetailRepo;

        // Get Inquiry's ViewModel
        /// <summary>
        /// Make details available in Details POST w/ BindProperty.
        /// </summary>
        [BindProperty]
        public InquiryViewModel InquiryVm { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="inqHeaderRepo"></param>
        /// <param name="inqDetailRepo"></param>
        public InquiryController(IInquiryHeaderRepository inqHeaderRepo, IInquiryDetailRepository inqDetailRepo)
        {
            _inqHeaderRepo = inqHeaderRepo;
            _inqDetailRepo = inqDetailRepo;
        }

        /// <summary>
        /// Index Page.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Inquiry Details.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Details(int id)
        {
            InquiryVm = new InquiryViewModel
            {
                InquiryHeader = _inqHeaderRepo.FirstOrDefault(u => u.Id == id),
                InquiryDetail = _inqDetailRepo.GetAll(u => u.InquiryHeaderId == id, includeProperties: "Product")
            };

            return View(InquiryVm);
        }

        /// <summary>
        /// Inquiry Details (populated)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            // Prepare a list for ShoppingCart.
            var shoppingCartList = new List<ShoppingCart>();

            // Since we already have the "current" inquiry header id captured in the hidden input in the Details view, we will just use that.
            InquiryVm.InquiryDetail = _inqDetailRepo.GetAll(u => u.InquiryHeaderId == InquiryVm.InquiryHeader.Id);

            // Now to add the inquiry details to the shopping cart, then a session.
            foreach (var detail in InquiryVm.InquiryDetail)
            {
                var shoppingCart = new ShoppingCart
                {
                    ProductId = detail.ProductId
                };

                // Now to add to the shopping cart list.
                shoppingCartList.Add(shoppingCart);
            }

            // Now to clear previous session, update session shopping cart list, and add inquiry session id.
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);

            // We need to know whether this session was set directly or using an inquiry.
            HttpContext.Session.Set(WebConstants.SessionInquiryId, InquiryVm.InquiryHeader.Id);

            TempData[WebConstants.Success] = "Items added to cart!";

            // Redirect to shopping cart after.
            return RedirectToAction("Index", "Cart");
        }

        /// <summary>
        /// Delete Inquiry.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete()
        {
            var inquiryHeader = _inqHeaderRepo.FirstOrDefault(u => u.Id == InquiryVm.InquiryHeader.Id); // Since we have the header id in the details view as a hidden val, we will use that.
            var inquiryDetails = _inqDetailRepo.GetAll(u => u.Id == InquiryVm.InquiryHeader.Id);

            // Use newly added RemoveRange() method from Repository to remove all of the inquiry details.
            _inqDetailRepo.RemoveRange(inquiryDetails);
            _inqHeaderRepo.Remove(inquiryHeader);
            // Header or details.save() will work as Save() does not depend on DbSet.
            _inqHeaderRepo.Save();
            TempData[WebConstants.Success] = "Inquiry deleted successfully!";

            return RedirectToAction(nameof(Index));
        }

        #region API CALLS

        // ToDo: separate this into its own Api Layer.
        /// <summary>
        /// Gets Inquiry List via API.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _inqHeaderRepo.GetAll() });
        }

        #endregion
    }
}