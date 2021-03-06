using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Models;
using ProjectStone_Models.ViewModels;
using ProjectStone_Utility;
using System;
using System.IO;

namespace ProjectStone.Controllers;

[Authorize(Roles = WebConstants.AdminRole)]
public class ProductController : Controller
{
    private readonly IProductRepository _productRepo;
    private readonly IWebHostEnvironment _webHostEnvironment;

    /// <summary>
    /// CTOR; Sets DI objects.
    /// </summary>
    /// <param name="productRepo"></param>
    /// <param name="webHostEnvironment"></param>
    public ProductController(IProductRepository productRepo, IWebHostEnvironment webHostEnvironment)
    {
        _productRepo = productRepo;
        _webHostEnvironment = webHostEnvironment; // Using DI to get webhostenv.
    }

    /// <summary>
    /// Product Page.
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        #region before eager loading (older)

        //IEnumerable<Product> productList = _db.Product;

        // To access Category, we need to load it in.
        //foreach (var obj in productList)
        //{
        // Foreach object in the product list, it will load the category model based on this condition (below)
        //obj.Category = _db.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
        //obj.SubCategory = _db.SubCategory.FirstOrDefault(u => u.Id == obj.SubCategoryTypeId);
        //}

        #endregion

        #region using eagar loading (old)

        // Replacing commented code block in region with this method of eagar loading.
        //IEnumerable<Product> productList = _db.Product.Include(u => u.Category).Include(u => u.SubCategory);

        #endregion

        #region using eagar loading with new Repository Pattern

        // Use the GetAll(...) params to invoke the .Include part of it.
        // Literally _db.Product.Include(u => u.Category).Include(u => u.SubCategory) vs _productRepo.GetAll(includeProperties:"Category,SubCategory")
        var productList = _productRepo.GetAll(includeProperties: "Category,SubCategory"); // No spaces in the "includeProperties:" string, everything will break.

        #endregion

        return View(productList);
    }

    /// <summary>
    /// GET - Upsert (Null for Create, int val for Edit)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IActionResult Upsert(int? id)
    {
        #region Before Repo Pattern

        //var productViewModel = new ProductViewModel
        //{
        //    Product = new Product(),
        //    CategorySelectList = _db.Category.Select(i => new SelectListItem
        //    {
        //        Text = i.Name,
        //        Value = i.Id.ToString()
        //    }),
        //    SubCategorySelectList = _db.SubCategory.Select(i => new SelectListItem
        //    {
        //        Text = i.Name,
        //        Value = i.Id.ToString()
        //    })
        //};

        #endregion

        // Use new method from ProductRepository. Two for the price of one.
        var productViewModel = new ProductViewModel
        {
            Product = new Product(),
            CategorySelectList = _productRepo.GetAllDropdownList(WebConstants.CategoryName),
            SubCategorySelectList = _productRepo.GetAllDropdownList(WebConstants.SubCategoryName)
        };

        if (id == null)
        {
            // To create a product.
            return View(productViewModel);
        }

        // To edit a product.
        productViewModel.Product = _productRepo.Find(id.GetValueOrDefault());

        if (productViewModel.Product == null) { return NotFound(); }

        return View(productViewModel);
    }

    /// <summary>
    /// POST - Upsert
    /// </summary>
    /// <param name="productViewModel"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(ProductViewModel productViewModel)
    {
        // Server-side validation.
        if (ModelState.IsValid)
        {
            var files = HttpContext.Request.Form.Files;
            var webRootPath = _webHostEnvironment.WebRootPath;

            if (productViewModel.Product.Id == 0)
            {
                // Creating.
                var uploadPath = webRootPath + WebConstants.ImagePath;
                var fileName = Guid.NewGuid().ToString(); // Rename default later, this will let the user pick a name regardless.
                var extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName + extension), FileMode.Create)) { files[0].CopyTo(fileStream); }

                // Name to store in the DB.
                productViewModel.Product.Image = fileName + extension;

                // Add the product.
                _productRepo.Add(productViewModel.Product);
            }
            else
            {
                // Updating.
                // AsNoTracking will tell EF not to track this entity to allow us to update the product if the image was not updated.
                // Pre Repo Pattern.
                //var productFromDb = _db.Product.AsNoTracking().FirstOrDefault(u => u.Id == productViewModel.Product.Id);

                // Using Repo Pattern.
                var productFromDb = _productRepo.FirstOrDefault(u => u.Id == productViewModel.Product.Id, isTracking: false);

                if (files.Count > 0)
                {
                    var uploadPath = webRootPath + WebConstants.ImagePath;
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);

                    var oldFile = Path.Combine(uploadPath, productFromDb.Image);

                    // Delete old image to make way for new one.
                    if (System.IO.File.Exists(oldFile)) { System.IO.File.Delete(oldFile); }

                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName + extension), FileMode.Create)) { files[0].CopyTo(fileStream); }

                    // Name to store in the DB.
                    productViewModel.Product.Image = fileName + extension;
                }
                else
                {
                    // Keep image from DB if it was not modified.
                    productViewModel.Product.Image = productFromDb.Image;
                }

                // Update the product.
                _productRepo.Update(productViewModel.Product);
            }

            // Save to DB.
            _productRepo.Save();
            TempData[WebConstants.Success] = "Action completed successfully!";

            return RedirectToAction("Index");
        }

        // To prevent any weird happenstances if the modelState is not valid, populate the CategoryList.
        productViewModel.CategorySelectList = _productRepo.GetAllDropdownList(WebConstants.CategoryName);
        productViewModel.SubCategorySelectList = _productRepo.GetAllDropdownList(WebConstants.SubCategoryName);
        TempData[WebConstants.Error] = "Error performing upsert action.";

        return View(productViewModel);
    }

    /// <summary>
    /// GET - Delete
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IActionResult Delete(int? id)
    {
        if (id is null or 0) { return NotFound(); }

        // "Include" category dropdown value when looking for product. (Eager loading)
        // Old.
        //var productFromDb = _db.Product.Include(u => u.Category).Include(u => u.SubCategory).FirstOrDefault(u => u.Id == id);

        // New.
        var productFromDb = _productRepo.FirstOrDefault(u => u.Id == id, "Category,SubCategory"); // "includeProperties:" was marked as redundant, so we will remove it from this line.

        if (productFromDb == null) { return NotFound(); }

        return View(productFromDb);
    }

    /// <summary>
    /// POST - Delete
    /// ActionName = Custom action name to let .NetCore know this is a Delete Action as well.  
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(int? id)
    {
        // Deleting.

        var productObj = _productRepo.Find(id.GetValueOrDefault());

        if (productObj == null) { NotFound(); }

        var uploadPath = _webHostEnvironment.WebRootPath + WebConstants.ImagePath;

        var oldFile = Path.Combine(uploadPath, productObj.Image);

        // Delete old image to make way for new one.
        if (System.IO.File.Exists(oldFile)) { System.IO.File.Delete(oldFile); }

        _productRepo.Remove(productObj);
        _productRepo.Save();
        TempData[WebConstants.Success] = "Action completed successfully!";

        return RedirectToAction("Index");
    }
}