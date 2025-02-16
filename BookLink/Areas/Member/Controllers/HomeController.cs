using BookLink.DataAccess.Repository;
using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using BookLink.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.Security.Claims;

namespace BookLink.Areas.Member.Controllers
{
	[Area("Member")]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<User> _userManager;


		public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, UserManager<User> userManager)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
			_userManager = userManager;
		}

		public IActionResult Index()
		{

			IEnumerable<Book> bookList = _unitOfWork.Book.GetAll(includeProperties: "BookCategory").ToList();

			return View(bookList);
		}

		// GET - Details
		public IActionResult Details(int bookId)
		{
			var book = _unitOfWork.Book.Get(b => b.BookId == bookId, includeProperties: "BookCategory");


			ShoppingCart cart = new()
			{
				Book = book,
				Count = 1,
				BookId = bookId,
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

			// Check if the user is an admin and prevent them from adding to the cart
			if (User.IsInRole(SD.Role_Admin))
			{
				TempData["error"] = "Admins cannot add items to the cart.";
				return RedirectToAction(nameof(Index));
			}


			shoppingCart.UserId = userId;

			ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get
				( u => u.UserId == userId 
				&& u.BookId == shoppingCart.BookId);
			
			if (cartFromDb!=null)
				// Shopping cart already exists
			{
				cartFromDb.Count += shoppingCart.Count;
				_unitOfWork.ShoppingCart.Update(cartFromDb);
				_unitOfWork.Save();
				TempData["success"] = "Item updated successfully!";
			}
			else
				// Shopping cart does not exist, ADD Cart
			{
				_unitOfWork.ShoppingCart.Add(shoppingCart);
				_unitOfWork.Save();
				TempData["success"] = "Item added to cart successfully!";
				HttpContext.Session.SetInt32(SD.SessionCart,
					_unitOfWork.ShoppingCart.GetAll(u => u.UserId == userId && u.BookId == shoppingCart.BookId).Count());
			}
			return RedirectToAction(nameof(Index));
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
