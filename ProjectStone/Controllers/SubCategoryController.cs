using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ProjectStone_DataAccess.Data;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Models;
using ProjectStone_Utility;

namespace ProjectStone.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class SubCategoryController : Controller
    {
        private readonly ISubCategoryRepository _subCategoryRepo;

        public SubCategoryController(ISubCategoryRepository subCategoryRepo)
        {
            _subCategoryRepo = subCategoryRepo;
        }
        public IActionResult Index()
        {
            var subCategoryList = _subCategoryRepo.GetAll();
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
        public IActionResult Create(SubCategory subCategoryObj)
        {
            if (ModelState.IsValid)
            {
                _subCategoryRepo.Add(subCategoryObj);
                _subCategoryRepo.Save();
                return Redirect("Index");
            }

            return View(subCategoryObj);
        }

        // GET - Edit
        public IActionResult Edit(int? id)
        {
            if (id is null or 0) { return NotFound(); }

            // Find only works on primary key.
            var subCategoryObj = _subCategoryRepo.Find(id.GetValueOrDefault());
            if (subCategoryObj is null) { return NotFound(); }

            return View(subCategoryObj);
        }

        // POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SubCategory subCategoryObj)
        {
            if (ModelState.IsValid)
            {
                _subCategoryRepo.Update(subCategoryObj);
                _subCategoryRepo.Save();

                return RedirectToAction("Index");
            }

            return View(subCategoryObj);
        }

        // GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id is null or 0) { return NotFound(); }

            var subCategoryObj = _subCategoryRepo.Find(id.GetValueOrDefault());
            if (subCategoryObj is null) { return NotFound(); }

            return View(subCategoryObj);
        }

        // POST - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var subCategoryObj = _subCategoryRepo.Find(id.GetValueOrDefault());
            
            if (subCategoryObj is null) { NotFound(); }

            _subCategoryRepo.Remove(subCategoryObj);
            _subCategoryRepo.Save();

            return RedirectToAction("Index");
        }
    }
}