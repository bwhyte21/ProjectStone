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

        public CategoryController(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

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

        // GET - Create
        public IActionResult Create()
        {
            return View();
        }

        // POST - Create
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

                // Redirect to index to display updated list.
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        // GET - Edit
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

        // POST - Edit
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

                return RedirectToAction("Index");
            }

            return View(obj);
        }

        // GET - Delete
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

        // POST - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            // Old
            //var obj = _db.Category.Find(id);

            //if (obj is null) { NotFound(); }

            //_db.Category.Remove(obj);
            //_db.SaveChanges();

            // New
            var categoryObj = _categoryRepo.Find(id.GetValueOrDefault());

            if (categoryObj == null) { NotFound(); }

            _categoryRepo.Remove(categoryObj);
            _categoryRepo.Save();

            return RedirectToAction("Index");
        }
    }
}