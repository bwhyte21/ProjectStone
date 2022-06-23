using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectStone_Models;

public class InquiryDetail
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int InquiryHeaderId { get; set; }

    [ForeignKey("InquiryHeaderId")]
    public InquiryHeader InquiryHeader { get; set; }

    [Required]
    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }
}