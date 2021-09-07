using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Models.ViewModels;
using ProjectStone_Utility;
using ProjectStone_Utility.BrainTree;
using System;
using System.Linq;

namespace ProjectStone.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IBrainTreeGate _brainTree;
        private readonly IOrderHeaderRepository _orderHeaderRepo;
        private readonly IOrderDetailRepository _orderDetailRepo;

        /// <summary>
        /// Bind OrderViewModel globally so we do not need to retrieve it for details and add it as a param. (for this example)
        /// </summary>
        [BindProperty]
        public OrderViewModel OrderVm { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="brainTree"></param>
        /// <param name="orderHeaderRepo"></param>
        /// <param name="orderDetailRepo"></param>
        public OrderController(IBrainTreeGate brainTree, IOrderHeaderRepository orderHeaderRepo, IOrderDetailRepository orderDetailRepo)
        {
            _brainTree = brainTree;
            _orderHeaderRepo = orderHeaderRepo;
            _orderDetailRepo = orderDetailRepo;
        }

        /// <summary>
        /// Parameters are for the Search Filter Form in the view via HtmlHelpers. 
        /// </summary>
        /// <param name="searchName"></param>
        /// <param name="searchEmail"></param>
        /// <param name="searchPhone"></param>
        /// <param name="OrderStatus"></param>
        /// <returns></returns>
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
                orderListViewModel.OrderHeadersList =
                    orderListViewModel.OrderHeadersList.Where(order => order.FullName.ToLower().Contains(searchName.ToLower())); // using toLower to eliminate case sensitivity.
            }

            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListViewModel.OrderHeadersList = orderListViewModel.OrderHeadersList.Where(order => order.Email.ToLower().Contains(searchEmail.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchPhone)) { orderListViewModel.OrderHeadersList = orderListViewModel.OrderHeadersList.Where(order => order.PhoneNumber.Contains(searchPhone)); }

            if (!string.IsNullOrEmpty(OrderStatus) && OrderStatus is not "--Order Status--")
            {
                orderListViewModel.OrderHeadersList = orderListViewModel.OrderHeadersList.Where(order => order.OrderStatus.Contains(OrderStatus));
            }

            // Pass view model to index view.
            return View(orderListViewModel);
        }

        /// <summary>
        /// Order Details page. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Details(int id)
        {
            OrderVm = new OrderViewModel
            {
                OrderHeader = _orderHeaderRepo.FirstOrDefault(order => order.Id == id),
                OrderDetail = _orderDetailRepo.GetAll(order => order.OrderHeader.Id == id, includeProperties: "Product")
            };

            return View(OrderVm);
        }

        /// <summary>
        /// Begin order process via Start Processing Btn.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult StartProcessing()
        {
            // Retrieve Order Header.
            var orderHeader = _orderHeaderRepo.FirstOrDefault(order => order.Id == OrderVm.OrderHeader.Id);

            // Update Order Status.
            orderHeader.OrderStatus = WebConstants.StatusInProcess;

            // Save to DB.
            _orderHeaderRepo.Save();

            // Alert user that order processing has begun (SweetAlert2)
            TempData[WebConstants.Success] = "Your order is being processed!";

            // Redirect to Index.
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Begin order shipping process.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ShipOrder()
        {
            // Retrieve Order Header.
            var orderHeader = _orderHeaderRepo.FirstOrDefault(order => order.Id == OrderVm.OrderHeader.Id);

            // Update Order Status.
            orderHeader.OrderStatus = WebConstants.StatusShipped;

            // Update shipping date.
            orderHeader.ShippingDate = DateTime.Now;

            // Save to DB.
            _orderHeaderRepo.Save();

            // Alert user that order was successfully shipped (SweetAlert2)
            TempData[WebConstants.Success] = "Your order is being shipped!";

            // Redirect to Index.
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Begin cancel order process using the BrainTree Gateway.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CancelOrder()
        {
            // Retrieve Order Header.
            var orderHeader = _orderHeaderRepo.FirstOrDefault(order => order.Id == OrderVm.OrderHeader.Id);

            // Get the BrainTree Gateway.
            var gateway = _brainTree.GetGateway();

            // Get transaction details via BrainTree using transactionId from OrderHeader.
            var transaction = gateway.Transaction.Find(orderHeader.TransactionId);

            // Check the status based on the transaction.
            // If the transaction is authorized or has been submitted for settlement, there will be no refund as the process has not been completed, and we do not have the money yet.
            // See https://developer.paypal.com/braintree/articles/get-started/transaction-lifecycle for more info.
            if (transaction.Status is TransactionStatus.AUTHORIZED or TransactionStatus.SUBMITTED_FOR_SETTLEMENT) // Logical Pattern (is, or)
            {
                // No Refund, Void Transaction.
                //var resultVoid = gateway.Transaction.Void(orderHeader.TransactionId); // Uncomment to watch variable.
                gateway.Transaction.Void(orderHeader.TransactionId);
            }
            else // Status is either Settling or Settled.
            {
                // Provide Refund.
                //var resultRefund = gateway.Transaction.Refund(orderHeader.TransactionId); // Uncomment to watch variable.
                gateway.Transaction.Refund(orderHeader.TransactionId);
            }

            // Update Order Status.
            orderHeader.OrderStatus = WebConstants.StatusRefunded;

            // Save to DB.
            _orderHeaderRepo.Save();

            // Alert user that order was successfully canceled (SweetAlert2)
            TempData[WebConstants.Success] = "You have successfully canceled your order!";

            // Redirect to Index.
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Update Order Details 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateOrderDetails()
        {
            // Retrieve Order Header.
            var orderHeaderFromDb = _orderHeaderRepo.FirstOrDefault(order => order.Id == OrderVm.OrderHeader.Id);

            // We only want to update certain fields from the form, so we will do it individually.
            orderHeaderFromDb.FullName = OrderVm.OrderHeader.FullName;
            orderHeaderFromDb.PhoneNumber = OrderVm.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVm.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVm.OrderHeader.City;
            orderHeaderFromDb.State = OrderVm.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVm.OrderHeader.PostalCode;
            orderHeaderFromDb.Email = OrderVm.OrderHeader.Email;

            // Save to DB.
            _orderHeaderRepo.Save();

            // Alert user that details were successfully updated (SweetAlert2)
            TempData[WebConstants.Success] = "Order Details successfully updated!";

            // Redirect back to Order Details page.
            return RedirectToAction(nameof(Details), new { id = orderHeaderFromDb.Id });
        }
    }
}