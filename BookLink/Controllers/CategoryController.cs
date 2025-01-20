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

		// GET: Display categories
		public IActionResult Index()
		{
			List<Category> categories = _context.Categories.ToList();
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
				return View("Index", _context.Categories.ToList());
			}

			_context.Categories.Add(category);
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}


		// GET: Get Category by ID for Editing
		[HttpGet]
		public IActionResult GetCategory(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			Category? category = _context.Categories.Find(id);
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
			_context.Categories.Update(category);
			_context.SaveChanges();
			return Json(new { success = true, message = "Category updated successfully" });
		}
	}
}
