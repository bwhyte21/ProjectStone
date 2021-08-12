using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ProjectStone_Models
{
  public class ApplicationUser : IdentityUser
  {
      public string FullName { get; set; }
  }
}
