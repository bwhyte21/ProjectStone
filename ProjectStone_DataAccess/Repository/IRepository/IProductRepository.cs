using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectStone_Models;
using System.Collections.Generic;

namespace ProjectStone_DataAccess.Repository.IRepository
{
  public interface IProductRepository : IRepository<Product>
    {
        void Update(Product obj);

        // New Method for SelectList. Two for the price of one.
        IEnumerable<SelectListItem> GetAllDropdownList(string obj);
    }
}