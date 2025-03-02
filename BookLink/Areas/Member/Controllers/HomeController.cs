using System.Threading.Tasks;
using BookLink.DataAccess.Repository;
using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using BookLink.Models.ViewModels;
using BookLink.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BookLink.Areas.Member.Controllers
{
	[Area("Member")]
	public class HomeController : Controller
	{
		#region Fields and Constructor

		private readonly ILogger<HomeController> _logger;
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<User> _userManager;

		public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, UserManager<User> userManager)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
			_userManager = userManager;
		}

		#endregion

		#region Primary Actions

		public async Task<IActionResult> Index()
		{
			var bookListQuery = _unitOfWork.Book.GetAllQuerable(includeProperties: "BookCategory");
			var totalItems = await bookListQuery.CountAsync();
			var items = await bookListQuery.Take(12).ToListAsync();

			var model = new BookSearchVM
			{
				Books = new PaginatedList<Book>(items, totalItems, 1, 12),
				SearchString = null,
				Category = null,
				TransactionType = null,
				Availability = null,
				Categories = GetCategories(),
				TransactionTypes = GetTransactionTypes(),
				AvailabilityOptions = GetAvailabilityOptions()
			};
			return View(model);
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

		[HttpPost]
		[Authorize]
		public IActionResult Details(ShoppingCart shoppingCart)
		{
			var userId = _userManager.GetUserId(User);

			// Check if the user is an admin and prevent them from adding to the cart
			if (User.IsInRole(SD.Role_Admin))
			{
				TempData["error"] = "Admins cannot add items to the cart.";
				return RedirectToAction(nameof(Index));
			}

			shoppingCart.UserId = userId;

			ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get
				(u => u.UserId == userId && u.BookId == shoppingCart.BookId);

			if (cartFromDb != null)
			{
				// Shopping cart already exists
				cartFromDb.Count += shoppingCart.Count;
				_unitOfWork.ShoppingCart.Update(cartFromDb);
				_unitOfWork.Save();
				TempData["success"] = "Item updated successfully!";
			}
			else
			{
				// Shopping cart does not exist, ADD Cart
				_unitOfWork.ShoppingCart.Add(shoppingCart);
				_unitOfWork.Save();
				TempData["success"] = "Item added to cart successfully!";
				HttpContext.Session.SetInt32(SD.SessionCart,
					_unitOfWork.ShoppingCart.GetAll(u => u.UserId == userId).Count());
			}
			return RedirectToAction(nameof(Index));
		}

		#endregion

		#region API Endpoints

		[HttpGet]
		public IActionResult SearchSuggestions(string term)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
					return Json(new List<object>());

				var cleanTerm = term.Trim().Replace("\"", "").Replace("*", "");
				var suggestions = _unitOfWork.Book
					.FullTextSearch(cleanTerm)
					.Select(b => new
					{
						label = b.Title,
						value = b.Title,
						id = b.BookId,
						author = b.Author,
						img = b.ImageUrl
					})
					.Distinct()
					.Take(10)
					.ToList();

				return Json(suggestions);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Search suggestions error");
				return Json(new List<object>());
			}
		}

		[HttpGet]
		public async Task<IActionResult> SearchResults(
			string searchString,
			string category,
			TransactionType? transactionType,
			AvailabilityStatus? availability,
			int page = 1,
			int pageSize = 12)
		{
			try
			{
				var bookListQuery = _unitOfWork.Book.GetAllQuerable(includeProperties: "BookCategory");

				bookListQuery = ApplySearchFilter(bookListQuery, searchString);
				bookListQuery = ApplyCategoryFilter(bookListQuery, category);
				bookListQuery = ApplyTransactionFilter(bookListQuery, transactionType);
				bookListQuery = ApplyAvailabilityFilter(bookListQuery, availability);

				var totalItems = await bookListQuery.CountAsync();
				var items = await bookListQuery
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ToListAsync();

				var model = new BookSearchVM
				{
					Books = new PaginatedList<Book>(items, totalItems, page, pageSize),
					SearchString = searchString,
					Category = category,
					TransactionType = transactionType,
					Availability = availability,
					Categories = GetCategories(),
					TransactionTypes = GetTransactionTypes(),
					AvailabilityOptions = GetAvailabilityOptions()
				};

				return PartialView("_BookResults", model);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Search error");
				return BadRequest("Search error");
			}
		}

		#endregion

		#region Filter Helpers

		private IQueryable<Book> ApplySearchFilter(IQueryable<Book> bookListQuery, string searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm)) return bookListQuery;
			return _unitOfWork.Book.FullTextSearch(searchTerm).Include(b => b.BookCategory).AsQueryable();
		}

		private IQueryable<Book> ApplyCategoryFilter(IQueryable<Book> bookListQuery, string category)
		{
			if (int.TryParse(category, out int categoryId))
				return bookListQuery.Where(b => b.BookCategory.CategoryId == categoryId);
			return bookListQuery;
		}

		private IQueryable<Book> ApplyTransactionFilter(IQueryable<Book> bookListQuery, TransactionType? transactionType)
		{
			return transactionType.HasValue ? bookListQuery.Where(b => b.TransactionType == transactionType.Value) : bookListQuery;
		}

		private IQueryable<Book> ApplyAvailabilityFilter(IQueryable<Book> bookListQuery, AvailabilityStatus? availability)
		{
			return availability switch
			{
				AvailabilityStatus.Available => bookListQuery.Where(b => b.BookStatus == BookStatus.Available),
				AvailabilityStatus.ComingSoon => bookListQuery.Where(b => b.BookStatus == BookStatus.Pending),
				_ => bookListQuery
			};
		}

		#endregion

		#region Dropdown Helpers

		private IEnumerable<SelectListItem> GetCategories()
		{
			return _unitOfWork.Category.GetAll()
				.Select(c => new SelectListItem { Text = c.CategoryName, Value = c.CategoryId.ToString() });
		}

		private IEnumerable<SelectListItem> GetTransactionTypes()
		{
			return Enum.GetValues(typeof(TransactionType))
				.Cast<TransactionType>()
				.Select(t => new SelectListItem { Text = t.ToString(), Value = ((int)t).ToString() });
		}

		private IEnumerable<SelectListItem> GetAvailabilityOptions()
		{
			return Enum.GetValues(typeof(AvailabilityStatus))
				.Cast<AvailabilityStatus>()
				.Select(a => new SelectListItem { Text = a.ToString(), Value = ((int)a).ToString() });
		}

		#endregion
	}
}