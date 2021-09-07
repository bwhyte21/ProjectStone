using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectStone_Models
{
  public class Product
    {
        public Product()
        {
            // Pre-set TempSqFt to 1 for the range.
            TempSqFt = 1;
        }

        [Key]
        public int Id { get; set; }

        [Required] // <-- Necessary for EF Core to alter the DB. Makes column in EF Not Nullable.
        public string Name { get; set; }

        public string ShortDesc { get; set; }

        public string Description { get; set; }

        [Range(1, int.MaxValue)]
        public double Price { get; set; }

        public string Image { get; set; }

        // Make EF aware of this foreign key. (Don't forget to add this to the DBContext to make the migration aware)
        [Display(Name = "Category Type")]
        public int CategoryId { get; set; }

        // By doing this.
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Display(Name = "SubCategory Type")]
        public int SubCategoryTypeId { get; set; }

        [ForeignKey("SubCategoryTypeId")]
        public virtual SubCategory SubCategory { get; set; }

        // Temp data for a specific user when they wish to add an item to the cart. This is only for the session and transaction log.
        [NotMapped] // Prevents this property from being added to the next EF migration.
        [Range(1, 10000, ErrorMessage = "SqFt MUST be greater than 0.")]
        public int TempSqFt { get; set; }
    }
}