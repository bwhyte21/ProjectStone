using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ProjectStone_Models.ViewModels
{
  public class OrderListViewModel
    {
        public IEnumerable<OrderHeader> OrderHeadersList { get; set; }

        public IEnumerable<SelectListItem> OrderStatusList { get; set; }

        public string OrderStatus { get; set; }
    }
}