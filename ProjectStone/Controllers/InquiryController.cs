using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Models.ViewModels;

namespace ProjectStone.Controllers
{
    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderRepository _inqHeaderRepo;
        private readonly IInquiryDetailRepository _inqDetailRepo;

        // Get Inquiry's ViewModel
        [BindProperty] // Make details available in Details POST w/ BindProperty.
        public InquiryViewModel InquiryVm { get; set; }

        public InquiryController(IInquiryHeaderRepository inqHeaderRepo, IInquiryDetailRepository inqDetailRepo)
        {
            _inqHeaderRepo = inqHeaderRepo;
            _inqDetailRepo = inqDetailRepo;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            InquiryVm = new InquiryViewModel
            {
                InquiryHeader = _inqHeaderRepo.FirstOrDefault(u => u.Id == id),
                InquiryDetail = _inqDetailRepo.GetAll(u => u.InquiryHeaderId == id, includeProperties: "Product")
            };
            
            return View(InquiryVm);
        }

        #region API Calls
        // ToDo: separate this into its own Api Layer.
        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _inqHeaderRepo.GetAll() });
        }

        #endregion
    }
}