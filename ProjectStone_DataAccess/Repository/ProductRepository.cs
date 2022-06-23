using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectStone_DataAccess.Data;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Models;
using ProjectStone_Utility;
using System.Collections.Generic;
using System.Linq;

namespace ProjectStone_DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _db;

    public ProductRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Product productObj)
    {
        // For SPECIFIC properties to be updated, use:
        //var objFromDb = FirstOrDefault(u => u.Id == productObj.Id);

        //if (objFromDb != null)
        //{
        //    objFromDb.Name = categoryObj.Name;
        //    ...
        //}

        // Another way to update. (Previous way is used in Category and SubCategory Repo classes)[above]
        // For ALL properties to be updated. Use explicit call:
        _db.Product.Update(productObj);
    }

    public IEnumerable<SelectListItem> GetAllDropdownList(string productObj)
    {
        #region Switch Statement Ver.

        // Before C# 8.0
        //switch (productObj)
        //{
        //    // If the obj is equal to Category, use the projections from the ProductController.
        //    case WebConstants.CategoryName:
        //        return _db.Category.Select(i => new SelectListItem
        //        {
        //            Text = i.Name,
        //            Value = i.Id.ToString()
        //        });
        //    // Same for SubCategory.
        //    case WebConstants.SubCategoryName:
        //        return _db.SubCategory.Select(i => new SelectListItem
        //        {
        //            Text = i.Name,
        //            Value = i.Id.ToString()
        //        });
        //    default:
        //        // Return null if both conditions are false.
        //        return null;
        //}

        #endregion

        #region Switch Expression Ver.

        // Utilizing C# 8.0's Switch Expression.

        return productObj switch
        {
            // If the obj is equal to Category, use the projections from the ProductController.
            WebConstants.CategoryName => _db.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }),
            // Same for SubCategory.
            WebConstants.SubCategoryName => _db.SubCategory.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }),
            _ => null
        };

        #endregion
    }
}