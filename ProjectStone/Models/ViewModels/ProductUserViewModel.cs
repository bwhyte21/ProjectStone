using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectStone.Models.ViewModels
{
  public class ProductUserViewModel
  {
      public ProductUserViewModel()
      {
          // Pre-assign product to product list to avoid future error.
          ProductList = new List<Product>();
      }
      public ApplicationUser ApplicationUser { get; set; }
      public IList<Product> ProductList { get; set; }
  }
}
