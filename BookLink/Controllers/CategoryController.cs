using BookLink.Data;
using BookLink.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookLink.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ApplicationDbContext _context;

		public CategoryController(ApplicationDbContext context)
		{
			_context = context;
		}

		// Displays a list of categories
		public IActionResult Index()
		{
			var categories = _context.Categories.ToList();
			return View(categories);
		}

		// Handles category creation
		[HttpPost]
		[ValidateAntiForgeryToken] // Prevents CSRF attacks
		public IActionResult Create(Category category)
		{
			if (!ModelState.IsValid)
			{
				// Return the same view with validation errors
				return View("Index", _context.Categories.ToList());
			}

			_context.Categories.Add(category);
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}
		
		
		[HttpGet]
		public JsonResult GetCategory(int id)
		{
			var category = _context.Categories.Find(id);
			if (category == null)
			{
				return Json(null);
			}
			return Json(category);
		}

		[HttpPost]
		[ValidateAntiForgeryToken] // Prevent CSRF attacks
		public JsonResult Edit(Category category)
		{
			if (!ModelState.IsValid)
			{
				return Json(new { success = false, message = "Invalid data" });
			}

			var existingCategory = _context.Categories.Find(category.CategoryId);
			if (existingCategory == null)
			{
				return Json(new { success = false, message = "Category not found" });
			}

			existingCategory.CategoryName = category.CategoryName;
			_context.SaveChanges();

			return Json(new { success = true });
		}














	}
}
