using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ProjectStone.Data;
using ProjectStone_Models;
using ProjectStone_Utility;

namespace ProjectStone.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var subCategoryList = _db.SubCategory;
            return View(subCategoryList);
        }

        // GET
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SubCategory obj)
        {
            if (ModelState.IsValid)
            {
                _db.SubCategory.Add(obj);
                _db.SaveChanges();
                return Redirect("Index");
            }

            return View(obj);
        }

        // GET - Edit
        public IActionResult Edit(int? id)
        {
            if (id is null or 0) { return NotFound(); }

            // Find only works on primary key.
            var obj = _db.SubCategory.Find(id);
            if (obj is null) { return NotFound(); }

            return View(obj);
        }

        // POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SubCategory obj)
        {
            if (ModelState.IsValid)
            {
                _db.SubCategory.Update(obj);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(obj);
        }

        // GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id is null or 0) { return NotFound(); }

            var obj = _db.SubCategory.Find(id);
            if (obj is null) { return NotFound(); }

            return View(obj);
        }

        // POST - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.SubCategory.Find(id);
            
            if (obj is null) { NotFound(); }

            _db.SubCategory.Remove(obj);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}