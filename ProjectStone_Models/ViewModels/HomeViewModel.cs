using System.Collections.Generic;

namespace ProjectStone_Models.ViewModels
{
  public class HomeViewModel
  {
      public IEnumerable<Product> Products { get; set; }
      public IEnumerable<Category> Categories { get; set; }
  }
}
