using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Models;
using ProjectStone_Utility;

namespace ProjectStone.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class CategoryController : Controller
    {
        // Use dependency injection to grab an instance for the data to set in the CTOR.
        //private readonly ApplicationDbContext _db;

        // Now that we have an IRepository/Repository setup for Category, we will use that instead of calling ApplicationDbContext outright. DEPENDENCY INJECTION!
        // This will enable more modularity for the project, changes only need happen in the Repository and NOT the controller(s).
        private readonly ICategoryRepository _categoryRepo;

        #region Previous CTOR using AppDbContext

        //public CategoryController(ApplicationDbContext db)
        //{
        //    // To be used throughout controller.
        //    _db = db;
        //}

        #endregion

        /// <summary>
        /// CTOR; Sets DI object.
        /// </summary>
        /// <param name="categoryRepo"></param>
        public CategoryController(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        /// <summary>
        /// Category page.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            #region Old Code

            // Old
            //IEnumerable<Category> categoryList = _db.Category;

            #endregion

            // New : we use the GetAll() instead since it's the Category Repo.
            var categoryList = _categoryRepo.GetAll();

            return View(categoryList);
        }

        /// <summary>
        /// GET - Create 
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST - Create
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            // Server-side validation.
            if (ModelState.IsValid)
            {
                #region Old Code

                // Pull object information from Create page.
                //_db.Category.Add(obj);

                // Add object to database using save changes.
                //_db.SaveChanges();

                #endregion

                // Now we must add objects thru the category repo.
                _categoryRepo.Add(obj);

                // and to save changes thru the category repo.
                _categoryRepo.Save();

                // Set TempData for Toastr to inform user of successful category creation.
                TempData[WebConstants.Success] = "Category created successfully!";

                // Redirect to index to display updated list.
                return RedirectToAction("Index");
            }

            // Set TempData for Toastr to inform user of unsuccessful category creation.
            TempData[WebConstants.Error] = "Error creating category.";

            return View(obj);
        }

        /// <summary>
        /// GET - Edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Edit(int? id)
        {
            if (id is null or 0) { return NotFound(); }

            // Find() only works on primary key.

            // Old
            //var obj = _db.Category.Find(id);
            //if (obj is null) { return NotFound(); }

            //return View(obj);

            // New w/ CategoryRepo
            var categoryObj = _categoryRepo.Find(id.GetValueOrDefault()); // Since the new Find() does not accept int?, we will use GetValueOrDefault()
            if (categoryObj == null) { return NotFound(); }

            return View(categoryObj);
        }

        /// <summary>
        /// POST - Edit
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                // Old
                //_db.Category.Update(obj);
                //_db.SaveChanges();

                // New
                _categoryRepo.Update(obj);
                _categoryRepo.Save();
                TempData[WebConstants.Success] = "Category modified successfully!";

                return RedirectToAction("Index");
            }

            TempData[WebConstants.Error] = "Error modifying category.";

            return View(obj);
        }

        /// <summary>
        /// GET - Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int? id)
        {
            if (id is null or 0) { return NotFound(); }

            // Old
            //var obj = _db.Category.Find(id);
            //if (obj is null) { return NotFound(); }

            //return View(obj);

            var categoryObj = _categoryRepo.Find(id.GetValueOrDefault());
            if (categoryObj == null) { return NotFound(); }

            return View(categoryObj);
        }

        /// <summary>
        /// POST - Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            #region Old

            //var obj = _db.Category.Find(id);

            //if (obj is null) { NotFound(); }

            //_db.Category.Remove(obj);
            //_db.SaveChanges();

            #endregion

            // New
            var categoryObj = _categoryRepo.Find(id.GetValueOrDefault());

            if (categoryObj == null) { NotFound(); }

            _categoryRepo.Remove(categoryObj);
            _categoryRepo.Save();
            TempData[WebConstants.Success] = "Category deleted successfully!";

            return RedirectToAction("Index");
        }
    }
}