using Microsoft.AspNetCore.Identity;

namespace ProjectStone_Models
{
  public class ApplicationUser : IdentityUser
  {
      public string FullName { get; set; }
  }
}
