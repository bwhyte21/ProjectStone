using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectStone_Models.ViewModels
{
  public class DetailsViewModel
  {
      // Init CTOR here in VW just in case it is forgotten in the Controller.
      public DetailsViewModel()
      {
          Product = new Product();
      }
      public Product Product { get; set; }
      public bool IsInCart { get; set; }
  }
}
