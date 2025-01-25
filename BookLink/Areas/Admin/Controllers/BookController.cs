using BookLink.DataAccess.Data;
using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookLink.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class BookController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public BookController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET: Display books
		public IActionResult Index()
		{
			List<Book> books = _unitOfWork.Book.GetAll().ToList();
			return View(books);
		}

	
		public ActionResult Create()
		{
			return View();
		}

		// Handles book creation
		[HttpPost]
		[ValidateAntiForgeryToken] // Prevents CSRF attacks
		public IActionResult Create(Book book)
		{
			if (!ModelState.IsValid)
			{
				return View(book);
			}
			_unitOfWork.Book.Add(book);
			_unitOfWork.Save();
			TempData["success"] = "Book created successfully";
			return RedirectToAction(nameof(Index));
		}

		// GET: Get Book by ID for Editing
		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			Book book = _unitOfWork.Book.Get(u => u.BookId == id);

			return book!= null ? View(book) : NotFound();
		}


		// Handles book editing
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(Book book)
		{
			if (!ModelState.IsValid)
			{
				return View(book);
			}

			_unitOfWork.Book.Update(book);
			_unitOfWork.Save();
			TempData["success"] = "Book updated successfully";
			return RedirectToAction(nameof(Index));
		}

		// GET: Get Book by ID for Deleting
		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			Book book = _unitOfWork.Book.Get(u => u.BookId == id);

			return book != null ? View(book) : NotFound();
		}

		// Handles book deletion
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult DeletePost(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			Book book = _unitOfWork.Book.Get(u => u.BookId == id);

			if (book == null)
			{
				return NotFound();
			}

			_unitOfWork.Book.Remove(book);
			_unitOfWork.Save();
			TempData["success"] = "Book deleted successfully";
			return RedirectToAction(nameof(Index));

		}


	}
}
