using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ProjectStone_Utility
{
    /// <summary>
    /// Moved into ProjectStone_Utility from Project Stone after library creation.
    /// Add the reference of this Utility project into the Main project (Project Stone)
    ///
    /// Eliminate the use of magic strings. Create constants here.
    /// </summary>
    public class WebConstants
    {
        // Access ImagePath.
        public const string ImagePath = @"\imgs\product\";

        // Access App Session.
        public const string SessionCart = "ShoppingCartSession";
        public const string SessionInquiryId = "InquirySession";

        // User Roles. (Made const from static to be used in [Authorize(Roles = ...)]
        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        // Admin Email.
        public const string AdminEmail = "bwhyte21@outlook.com";

        // Product Repository SelectListItems.
        public const string CategoryName = "Category";
        public const string SubCategoryName = "SubCategory";

        // Tool Notification Constants.
        public const string Success = "Success";
        public const string Error = "Error";

        // Order Status.
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        // Create a collection (IEnumerable) of strings to list the Order Statuses to be used in the Order Controller.
        // MUST be a ReadOnly Collection in order to create a list in this class.
        public static readonly IEnumerable<string> StatusList = new ReadOnlyCollection<string>(new List<string>
        {
            StatusApproved,
            StatusCancelled,
            StatusInProcess,
            StatusPending,
            StatusRefunded,
            StatusShipped
        });
    }
}