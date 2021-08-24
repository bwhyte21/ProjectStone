using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProjectStone_Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        
        [NotMapped] // We use NotMapped to avoid adding it to the DB via Migrations.
        public string Address { get; set; }
        [NotMapped]
        public string City { get; set; }
        [NotMapped]
        public string State { get; set; }
        [NotMapped]
        public string PostalCode { get; set; }
    }
}