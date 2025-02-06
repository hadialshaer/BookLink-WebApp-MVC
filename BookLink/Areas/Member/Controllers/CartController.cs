using Microsoft.AspNetCore.Mvc;

namespace BookLink.Areas.Member.Controllers
{
	public class CartController : Controller
	{
		[Area("Member")]
		public IActionResult Index()
		{
			return View();
		}
	}
}
