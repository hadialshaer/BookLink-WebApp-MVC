using BookLink.Data;
using BookLink.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookLink.Controllers
{
	// Controller for handling Category-related actions
	public class CategoryController : Controller
	{
		// Database context to interact with the database
		private readonly ApplicationDbContext _db;

		// Constructor to inject the database context
		public CategoryController(ApplicationDbContext db)
		{
			_db = db;
		}

		// Action to display the list of categories
		public IActionResult Index()
		{
			// Retrieve all categories from the database and pass them to the view
			List<Category> objCategoryList = _db.Categories.ToList();
			return View(objCategoryList);
		}

		// Action to handle the creation of a new category
		[HttpPost]
		[ValidateAntiForgeryToken] // Prevents Cross-Site Request Forgery (CSRF) attacks
		public IActionResult Create(Category category)
		{
			//We can add here custom validation
			//ex: if(..........)ModelState.AddModelError("CategoryName", "Category Name is required");
			// Server-side validation of the model
			if (ModelState.IsValid)
			{
				// Add the new category to the database
				_db.Categories.Add(category);
				_db.SaveChanges(); // Persist changes to the database
				return RedirectToAction("Index"); // Redirect back to the Index action
			}

			// If validation fails, retrieve the updated list of categories
			List<Category> updatedCategories = _db.Categories.ToList();

			// Return the Index view with the updated category list and validation errors
			return View("Index", updatedCategories);
		}
	}
}