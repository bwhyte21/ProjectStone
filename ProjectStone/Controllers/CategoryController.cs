using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectStone.Data;
using ProjectStone.Models;

namespace ProjectStone.Controllers
{
    public class CategoryController : Controller
    {
        // Use dependency injection to grab an instance for the data to set in the CTOR.
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            // To be used throughout controller.
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categoryList = _db.Category;

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
                // Pull object information from Create page.
                _db.Category.Add(obj);

                // Add object to database using save changes.
                _db.SaveChanges();

                // Redirect to index to display updated list.
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        // GET - Edit
        public IActionResult Edit(int? id)
        {
            if (id==null || id ==0)
            {
                return NotFound();
            }

            // Find only works on primary key.
            var obj = _db.Category.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            
            return View(obj);
        }
    }
}