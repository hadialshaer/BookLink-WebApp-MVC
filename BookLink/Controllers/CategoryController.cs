using AspNetCoreGeneratedDocument;
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
		// GET: Category
		public IActionResult Index()
		{
			List<Category> objCategoryList = _db.Categories.ToList();
			return View(objCategoryList);
		}

		public IActionResult Create()
		{
			return PartialView("_CreateCategoryPartial");
		}

		[HttpPost]
		public IActionResult Create(Category category)
		{
			_db.Categories.Add(category); // Keep Track of changes made but not actually added them
			_db.SaveChanges(); // Save Changes to the Database (create the category)
			return RedirectToAction("Index"); // Redirect to the Index Action of the Category Controller
		}

	}
}
