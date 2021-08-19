using System.Collections.Generic;

namespace ProjectStone_Models.ViewModels
{
  public class InquiryViewModel
  {
      public InquiryHeader InquiryHeader { get; set; }
      public IEnumerable<InquiryDetail> InquiryDetail { get; set; }
  }
}
