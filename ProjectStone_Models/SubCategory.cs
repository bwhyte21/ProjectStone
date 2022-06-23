using System.ComponentModel.DataAnnotations;

namespace ProjectStone_Models;

public class SubCategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
}