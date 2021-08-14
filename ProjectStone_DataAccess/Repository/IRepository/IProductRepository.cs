using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectStone_Models;

namespace ProjectStone_DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product obj);

        // New Method for SelectList. Two for the price of one.
        IEnumerable<SelectListItem> GetAllDropdownList(string obj);
    }
}