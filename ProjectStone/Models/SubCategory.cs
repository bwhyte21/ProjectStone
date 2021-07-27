﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectStone.Models
{
  public class SubCategory
  {
      [Key]
      public int Id { get; set; }

      public string Name { get; set; }
  }
}
