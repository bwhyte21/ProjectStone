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

        public OrderController(IBrainTreeGate brainTree, IOrderHeaderRepository orderHeaderRepo, IOrderDetailRepository orderDetailRepo)
        {
            _brainTree = brainTree;
            _orderHeaderRepo = orderHeaderRepo;
            _orderDetailRepo = orderDetailRepo;
        }
        public IActionResult Index()
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

            // Pass view model to index view.
            return View(orderListViewModel);
        }
    }
}