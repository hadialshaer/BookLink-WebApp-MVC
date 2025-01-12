using BookLink.Data;
using BookLink.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookLink.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ApplicationDbContext _db;

		public CategoryController(ApplicationDbContext db)
		{
			_db = db;
		}

		public IActionResult Index()
		{
			List<Category> objCategoryList = _db.Categories.ToList();
			return View(objCategoryList);
		}

		[HttpPost]
		public IActionResult Create(Category category)
		{
			if (ModelState.IsValid)
			{
				_db.Categories.Add(category);
				_db.SaveChanges();
				return RedirectToAction("Index");
			}

			// Return the partial view with validation errors
			return PartialView("_CreateCategoryPartial");
		}
	}
}
