using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Models;
using ProjectStone_Utility;

namespace ProjectStone.Controllers;

[Authorize(Roles = WebConstants.AdminRole)]
public class SubCategoryController : Controller
{
    private readonly ISubCategoryRepository _subCategoryRepo;

    /// <summary>
    /// CTOR; Sets DI object.
    /// </summary>
    /// <param name="subCategoryRepo"></param>
    public SubCategoryController(ISubCategoryRepository subCategoryRepo)
    {
        _subCategoryRepo = subCategoryRepo;
    }

    /// <summary>
    /// SubCategory Page.
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        var subCategoryList = _subCategoryRepo.GetAll();

        return View(subCategoryList);
    }

    /// <summary>
    /// GET - Create SubCategory Page.
    /// </summary>
    /// <returns></returns>
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// POST - Post Created Category.
    /// </summary>
    /// <param name="subCategoryObj"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(SubCategory subCategoryObj)
    {
        if (ModelState.IsValid)
        {
            _subCategoryRepo.Add(subCategoryObj);
            _subCategoryRepo.Save();
            TempData[WebConstants.Success] = "SubCategory created successfully!";

            return Redirect("Index");
        }

        TempData[WebConstants.Error] = "Error creating SubCategory.";

        return View(subCategoryObj);
    }

    /// <summary>
    /// GET - Edit - Edit SubCategory.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IActionResult Edit(int? id)
    {
        if (id is null or 0) { return NotFound(); }

        // Find only works on primary key.
        var subCategoryObj = _subCategoryRepo.Find(id.GetValueOrDefault());
        if (subCategoryObj == null) { return NotFound(); }

        return View(subCategoryObj);
    }

    /// <summary>
    /// POST - Edit - Posted Edited SubCategory. 
    /// </summary>
    /// <param name="subCategoryObj"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(SubCategory subCategoryObj)
    {
        if (ModelState.IsValid)
        {
            _subCategoryRepo.Update(subCategoryObj);
            _subCategoryRepo.Save();
            TempData[WebConstants.Success] = "SubCategory modified successfully!";

            return RedirectToAction("Index");
        }

        TempData[WebConstants.Error] = "Error modifying SubCategory.";

        return View(subCategoryObj);
    }

    /// <summary>
    /// GET - Delete - Delete SubCategory Page.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IActionResult Delete(int? id)
    {
        if (id is null or 0) { return NotFound(); }

        var subCategoryObj = _subCategoryRepo.Find(id.GetValueOrDefault());
        if (subCategoryObj == null) { return NotFound(); }

        return View(subCategoryObj);
    }

    /// <summary>
    /// POST - Delete - Delete SubCategory.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(int? id)
    {
        var subCategoryObj = _subCategoryRepo.Find(id.GetValueOrDefault());

        if (subCategoryObj == null) { NotFound(); }

        _subCategoryRepo.Remove(subCategoryObj);
        _subCategoryRepo.Save();
        TempData[WebConstants.Success] = "SubCategory deleted successfully!";

        return RedirectToAction("Index");
    }
}