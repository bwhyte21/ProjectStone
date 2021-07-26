using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectStone.Models
{
  public class Category
  {
      // Data annotations "[Key]" for table to identify primary key. To keep it explicit, use [Key].
      [Key]
      public int Id { get; set; }

      public string Name { get; set; }
      public int DisplayOrder { get; set; }
  }
}
