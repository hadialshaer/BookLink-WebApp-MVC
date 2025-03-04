using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BookLink.Areas.Member.Controllers
{
	[Area("Member")]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<User> _userManager;
		private readonly IMemoryCache _memoryCache;

		public HomeController(
			ILogger<HomeController> logger,
			IUnitOfWork unitOfWork,
			UserManager<User> userManager,
			IMemoryCache memoryCache)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_memoryCache = memoryCache;
		}

		public IActionResult Index()
		{
			var bookListQuery = _unitOfWork.Book.GetAllQuerable(includeProperties: "BookCategory");
			var totalItems = bookListQuery.Count();
			var items = bookListQuery.Take(12).ToList();

			var model = new BookSearchVM
			{
				Books = new PaginatedList<Book>(items, totalItems, 1, 12),
				SearchString = null,
				Category = null,
				TransactionType = null,
				Availability = null,
				Categories = GetCategories(),
				TransactionTypes = GetTransactionTypes(),
				AvailabilityOptions = GetAvailabilityOptions(),
				WishlistBookIds = GetWishlistBookIds()
			};
			return View(model);
		}

		public IActionResult Details(int bookId)
		{
			var book = _unitOfWork.Book.Get(b => b.BookId == bookId, includeProperties: "BookCategory");
			if (book == null)
			{
				TempData["error"] = "Book not found.";
				return RedirectToAction(nameof(Index));
			}

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
				cartFromDb.Count += shoppingCart.Count;
				_unitOfWork.ShoppingCart.Update(cartFromDb);
				_unitOfWork.Save();
				TempData["success"] = "Item updated successfully!";
			}
			else
			{
				_unitOfWork.ShoppingCart.Add(shoppingCart);
				_unitOfWork.Save();
				TempData["success"] = "Item added to cart successfully!";
				HttpContext.Session.SetInt32(SD.SessionCart,
					_unitOfWork.ShoppingCart.GetAll(u => u.UserId == userId).Count());
			}
			return RedirectToAction(nameof(Index));
		}

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
		public IActionResult SearchResults(
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

				var totalItems = bookListQuery.Count();
				var items = bookListQuery
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ToList();

				var model = new BookSearchVM
				{
					Books = new PaginatedList<Book>(items, totalItems, page, pageSize),
					SearchString = searchString,
					Category = category,
					TransactionType = transactionType,
					Availability = availability,
					Categories = GetCategories(),
					TransactionTypes = GetTransactionTypes(),
					AvailabilityOptions = GetAvailabilityOptions(),
					WishlistBookIds = GetWishlistBookIds()
				};

				return PartialView("_BookResults", model);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Search error");
				return BadRequest("Search error");
			}
		}

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
				AvailabilityStatus.ComingSoon => bookListQuery.Where(b => b.BookStatus == BookStatus.Borrowed),
				_ => bookListQuery
			};
		}

		private const string CategoriesCacheKey = "BookCategories";

		private IEnumerable<SelectListItem> GetCategories()
		{
			if (!_memoryCache.TryGetValue(CategoriesCacheKey, out IEnumerable<SelectListItem> cachedCategories))
			{
				cachedCategories = _unitOfWork.Category.GetAll()
					.Select(c => new SelectListItem
					{
						Text = c.CategoryName,
						Value = c.CategoryId.ToString()
					})
					.ToList();

				var cacheOptions = new MemoryCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
					SlidingExpiration = TimeSpan.FromMinutes(5)
				};

				_memoryCache.Set(CategoriesCacheKey, cachedCategories, cacheOptions);
			}
			return cachedCategories;
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

		private List<int> GetWishlistBookIds()
		{
			if (!User.Identity.IsAuthenticated)
			{
				return new List<int>();
			}

			var userId = _userManager.GetUserId(User);
			return _unitOfWork.Wishlist.GetAll(w => w.UserId == userId)
				.Select(w => w.BookId)
				.ToList();
		}
	}
}