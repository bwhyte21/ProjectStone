using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectStone_Models.ViewModels
{
  public class InquiryViewModel
  {
      public InquiryHeader InquiryHeader { get; set; }
      public IEnumerable<InquiryDetail> InquiryDetail { get; set; }
  }
}
