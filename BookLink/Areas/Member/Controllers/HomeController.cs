using BookLink.DataAccess.Repository;
using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
