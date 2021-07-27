using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectStone.Data;
using ProjectStone.Models;

namespace ProjectStone.Controllers
{
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
            _db.SubCategory.Add(obj);
            _db.SaveChanges();

            return Redirect("Index");
        }
    }
}