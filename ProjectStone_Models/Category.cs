using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectStone_Models;

public class Category
{
    // Data annotations "[Key]" for table to identify primary key. To keep it explicit, use [Key].
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    // Using Data Annotations change text format.
    [DisplayName("Display Order")]
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Display Order for category must be greater than 0!")]
    public int DisplayOrder { get; set; }
}