using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectStone_Models.ViewModels
{
  public class HomeViewModel
  {
      public IEnumerable<Product> Products { get; set; }
      public IEnumerable<Category> Categories { get; set; }
  }
}
