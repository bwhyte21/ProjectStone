using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectStone_Models;

public class OrderDetail
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int OrderHeaderId { get; set; }

    [ForeignKey("OrderHeaderId")]
    public OrderHeader OrderHeader { get; set; }

    [Required]
    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }

    public int SqFt { get; set; }
    public double PricePerSqFt { get; set; }
}