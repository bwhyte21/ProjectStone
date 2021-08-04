using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectStone.Data;
using ProjectStone.Models;
using ProjectStone.Models.ViewModels;

namespace ProjectStone.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment; // Using DI to get webhostenv.
        }

        public IActionResult Index()
        {
            #region before eager loading
            //IEnumerable<Product> productList = _db.Product;

            // To access Category, we need to load it in.
            //foreach (var obj in productList)
            //{
                // Foreach object in the product list, it will load the category model based on this condition (below)
                //obj.Category = _db.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
                //obj.SubCategory = _db.SubCategory.FirstOrDefault(u => u.Id == obj.SubCategoryTypeId);
            //}
            #endregion
         
            #region using eagar loading
            
            // Replacing commented code block in region with this method of eagar loading.
            IEnumerable<Product> productList = _db.Product.Include(u => u.Category).Include(u => u.SubCategory);
            
            #endregion

            return View(productList);
        }

        // GET - Upsert (Null for Create, int val for Edit)
        public IActionResult Upsert(int? id)
        {
            var productViewModel = new ProductViewModel
            {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                SubCategorySelectList = _db.SubCategory.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if (id is null)
            {
                // To create a product.
                return View(productViewModel);
            }

            // To edit a product.
            productViewModel.Product = _db.Product.Find(id);

            if (productViewModel.Product is null) { return NotFound(); }

            return View(productViewModel);
        }

        // POST - Create
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
                    _db.Product.Add(productViewModel.Product);
                }
                else
                {
                    // Updating.
                    // AsNoTracking will tell EF not to track this entity to allow us to update the product if the image was not updated.
                    var productFromDb = _db.Product.AsNoTracking().FirstOrDefault(u => u.Id == productViewModel.Product.Id);

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
                    _db.Product.Update(productViewModel.Product);
                }

                // Save to DB.
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            // To prevent any weird happenstances if the modelState is not valid, populate the CategoryList.
            productViewModel.CategorySelectList = _db.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            
            productViewModel.SubCategorySelectList = _db.SubCategory.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            return View(productViewModel);
        }

        // GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id is null or 0) { return NotFound(); }

            // "Include" category dropdown value when looking for product. (Eager loading)
            var productFromDb = _db.Product.Include(u => u.Category).Include(u => u.SubCategory).FirstOrDefault(u => u.Id == id);

            if (productFromDb is null) { return NotFound(); }

            return View(productFromDb);
        }

        // POST - Delete
        // ActionName = Custom action name to let .NetCore know this is a Delete Action as well.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            // Deleting.

            var productObj = _db.Product.Find(id);

            if (productObj is null) { NotFound(); }

            var uploadPath = _webHostEnvironment.WebRootPath + WebConstants.ImagePath;

            var oldFile = Path.Combine(uploadPath, productObj.Image);

            // Delete old image to make way for new one.
            if (System.IO.File.Exists(oldFile)) { System.IO.File.Delete(oldFile); }

            _db.Product.Remove(productObj);

            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}