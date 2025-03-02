using BookLink.DataAccess.Data;
using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using BookLink.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BookLink.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class CategoryController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMemoryCache _cache;
		public CategoryController(IUnitOfWork unitOfWork, IMemoryCache cache)
		{
			_unitOfWork = unitOfWork;
			_cache = cache;
		}

		// GET: Display categories
		public IActionResult Index()
		{
			List<Category> categories = _unitOfWork.Category.GetAll().ToList();
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
				return View(nameof(Index), _unitOfWork.Category.GetAll().ToList());
			}

			_unitOfWork.Category.Add(category);
			_unitOfWork.Save();

			TempData["success"] = "Category created succesfully";

			_cache.Remove("CategoriesCacheKey");
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

			Category? category = _unitOfWork.Category.Get(u => u.CategoryId == id);
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
			_unitOfWork.Category.Update(category);
			_unitOfWork.Save();

			TempData["success"] = "Category updated successfully";
			_cache.Remove("CategoriesCacheKey");
			return Json(new { success = true, message = "Category updated successfully" });
		}


		// POST: Delete Category
		[HttpPost]
		[ValidateAntiForgeryToken] // Prevent CSRF attacks
		public IActionResult Delete(int? id)
		{
			Category? category = _unitOfWork.Category.Get(u => u.CategoryId == id);

			if (category == null)
			{
				return NotFound();
			}
			_unitOfWork.Category.Remove(category);
			_unitOfWork.Save();

			TempData["success"] = "Category deleted succesfully"; // TempData is used to store temporary data, works for only one request
			_cache.Remove("CategoriesCacheKey");
			return RedirectToAction(nameof(Index));
		}

	}
}
