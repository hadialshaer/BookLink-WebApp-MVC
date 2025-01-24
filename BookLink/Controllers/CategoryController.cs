using BookLink;
using BookLink.DataAccess.Data;
using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookLink.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ICategoryRepository _categoryRep;

		public CategoryController(ICategoryRepository context)
		{
			_categoryRep = context;
		}

		// GET: Display categories
		public IActionResult Index()
		{
			List<Category> categories = _categoryRep.GetAll().ToList();
			return View(categories);
		}

		// Handles category creation
		[HttpPost]
		[ValidateAntiForgeryToken] // Prevents CSRF attacks
		public IActionResult Create(Category category)
		{
			//Custom Validation 
			//if (category.CategoryName == "")
			//{
			//	ModelState.AddModelError("CategoryName", "Category Name cannot be empty spaces");
			//}

			if (!ModelState.IsValid) // Server side validation 2: Check if the model is valid
			{
				// Return the same view with validation errors
				return View(nameof(Index), _categoryRep.GetAll().ToList());
			}

			_categoryRep.Add(category);
			_categoryRep.Save();

			TempData["success"] = "Category created succesfully";

			return RedirectToAction(nameof(Index));
		}


		// GET: Get Category by ID for Editing
		[HttpGet]
		public IActionResult GetCategory(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			Category? category = _categoryRep.Get(u=>u.CategoryId == id);
			if (category == null)
			{
				return Json(new { success = false, message = "Category not found" });
			}

			return Json(category);
		}

		// POST: Update Category
		[HttpPost]
		[ValidateAntiForgeryToken] // Prevent CSRF attacks
		public IActionResult Edit([FromBody] Category category)
		{
			if (!ModelState.IsValid)
			{
				return Json(new { success = false, message = "Invalid data" });
			}
			_categoryRep.Update(category);
			_categoryRep.Save();

			TempData["success"] = "Category updated successfully";

			return Json(new { success = true, message = "Category updated successfully" });
		}


		// POST: Delete Category
		[HttpPost]
		[ValidateAntiForgeryToken] // Prevent CSRF attacks
		public IActionResult Delete(int? id)
		{
			Category? category = _categoryRep.Get(u => u.CategoryId == id);

			if (category == null) 
			{
				return NotFound();
			}
			_categoryRep.Remove(category);
			_categoryRep.Save();

			TempData["success"] = "Category deleted succesfully"; // TempData is used to store temporary data, works for only one request
			
			return RedirectToAction(nameof(Index));
		}

	}
}
