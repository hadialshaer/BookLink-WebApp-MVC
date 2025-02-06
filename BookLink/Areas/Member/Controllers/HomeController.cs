using BookLink.DataAccess.Repository;
using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookLink.Areas.Member.Controllers
{
	[Area("Member")]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		private readonly IUnitOfWork _unitOfWork;


		public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{

			IEnumerable<Book> bookList = _unitOfWork.Book.GetAll(includeProperties: "BookCategory").ToList();

			return View(bookList);
		}

		// GET - Details
		public IActionResult Details(int bookId)
		{
			ShoppingCart cart = new()
			{
				Book = _unitOfWork.Book.Get(u => u.BookId == bookId, includeProperties: "BookCategory"),
				Count = 1,
				BookId = bookId
			};

			return View(cart);
		}

		public IActionResult About()
		{
			return View();
		}

		[HttpPost] 
		[Authorize]
		public IActionResult Details(ShoppingCart shoppingCart)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			shoppingCart.UserId = userId;

			ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get
				( u => u.UserId == userId 
				&& u.BookId == shoppingCart.BookId);
			
			if (cartFromDb!=null)
				// Shopping cart already exists
			{
				cartFromDb.Count += shoppingCart.Count;
				_unitOfWork.ShoppingCart.Update(cartFromDb);
				TempData["success"] = "Item updated successfully!";
			}
			else
				// Shopping cart does not exist, ADD Cart
			{
				_unitOfWork.ShoppingCart.Add(shoppingCart);
				TempData["success"] = "Item added to cart successfully!";
			}

			

			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
