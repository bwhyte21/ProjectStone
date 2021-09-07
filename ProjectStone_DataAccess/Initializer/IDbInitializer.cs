using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectStone_DataAccess.Initializer
{
    /// <summary>
    /// An interface for a Database Initializer for initial DB setup to create user roles and admin users for project deployment to a new environment.
    /// </summary>
  public interface IDbInitializer
  {
      void Initialize();
  }
}
