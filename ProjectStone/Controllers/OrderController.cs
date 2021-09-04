using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Models.ViewModels;
using ProjectStone_Utility;
using ProjectStone_Utility.BrainTree;

namespace ProjectStone.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IBrainTreeGate _brainTree;
        private readonly IOrderHeaderRepository _orderHeaderRepo;
        private readonly IOrderDetailRepository _orderDetailRepo;

        // Bind OrderViewModel globally so we do not need to retrieve it for details and add it as a param. (for this example)
        [BindProperty]
        public OrderViewModel OrderVm { get; set; }

        public OrderController(IBrainTreeGate brainTree, IOrderHeaderRepository orderHeaderRepo, IOrderDetailRepository orderDetailRepo)
        {
            _brainTree = brainTree;
            _orderHeaderRepo = orderHeaderRepo;
            _orderDetailRepo = orderDetailRepo;
        }

        // Parameters are for the Search Filter Form in the view via HtmlHelpers.
        public IActionResult Index(string searchName = null, string searchEmail = null, string searchPhone = null, string OrderStatus = null)
        {
            // Populate new VM.
            var orderListViewModel = new OrderListViewModel
            {
                OrderHeadersList = _orderHeaderRepo.GetAll(),
                // Using "projections" to populate the select list.
                OrderStatusList = WebConstants.statusList.ToList().Select(item => new SelectListItem
                {
                    Text = item,
                    Value = item
                })
            };

            // Check search filter form params for search queries.
            if (!string.IsNullOrEmpty(searchName))
            {
                orderListViewModel.OrderHeadersList = orderListViewModel.OrderHeadersList.Where(order => order.FullName.ToLower().Contains(searchName.ToLower())); // using toLower to eliminate case sensitivity.
            }

            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListViewModel.OrderHeadersList = orderListViewModel.OrderHeadersList.Where(order => order.Email.ToLower().Contains(searchEmail.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchPhone))
            {
                orderListViewModel.OrderHeadersList = orderListViewModel.OrderHeadersList.Where(order => order.PhoneNumber.Contains(searchPhone));
            }

            if (!string.IsNullOrEmpty(OrderStatus) && OrderStatus is not "--Order Status--")
            {
                orderListViewModel.OrderHeadersList = orderListViewModel.OrderHeadersList.Where(order => order.OrderStatus.Contains(OrderStatus));
            }

            // Pass view model to index view.
            return View(orderListViewModel);
        }

        // Order Details page.
        public IActionResult Details(int id)
        {
            OrderVm = new OrderViewModel
            {
                OrderHeader = _orderHeaderRepo.FirstOrDefault(order => order.Id == id),
                OrderDetail = _orderDetailRepo.GetAll(order => order.OrderHeader.Id == id, includeProperties: "Product")
            };

            return View(OrderVm);
        }
    }
}