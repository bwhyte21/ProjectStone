using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectStone_Utility
{
    /// <summary>
    /// Moved into ProjectStone_Utility from Project Stone after library creation.
    /// Add the reference of this Utility project into the Main project (Project Stone)
    /// </summary>
    public class WebConstants
    {
        // Access ImagePath.
        public const string ImagePath = @"\imgs\product\";

        // Access App Session.
        public const string SessionCart = "ShoppingCartSession";

        // User Roles. (Made const from static to be used in [Authorize(Roles = ...)]
        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        // Admin Email.
        public const string AdminEmail = "bwhyte21@outlook.com";
    }
}