using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectStone_Models;

public class InquiryHeader
{
    [Key] // Primary Key for EF to recognize.
    public int Id { get; set; }

    public string ApplicationUserId { get; set; }

    [ForeignKey("ApplicationUserId")]
    public ApplicationUser ApplicationUser { get; set; }

    public DateTime InquiryDate { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    public string Email { get; set; }
}