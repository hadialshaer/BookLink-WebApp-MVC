using Microsoft.AspNetCore.Mvc;

namespace BookLink.Controllers
{
	public class CategoryController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
