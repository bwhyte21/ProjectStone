using System.Collections.Generic;

namespace ProjectStone_Models.ViewModels
{
  public class OrderViewModel
  {
      public OrderHeader OrderHeader { get; set; }
      public IEnumerable<OrderDetail> OrderDetail { get; set; }
  }
}
