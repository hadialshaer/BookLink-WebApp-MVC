using BookLink.DataAccess.Data;
using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using BookLink.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

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


		public ActionResult Upsert(int? id)
		{
			BookVM bookVM = new()
			{
				CategoryList = _unitOfWork.Category.GetAll().Select(
				u => new SelectListItem
				{
					Text = u.CategoryName,
					Value = u.CategoryId.ToString()
				}), // Get all categories and convert them to SelectListItem

				Book = new Book()
			};

			if (id == null || id == 0)
			{
				// create
				return View(bookVM);
			}
			else
			{
				//  update
				bookVM.Book = _unitOfWork.Book.Get(u => u.BookId == id);
				return bookVM.Book != null ? View(bookVM) : NotFound();
			}
		}

		// Handles book creation
		[HttpPost]
		[ValidateAntiForgeryToken] // Prevents CSRF attacks
		public IActionResult Upsert(BookVM bookVM, IFormFile? file)
		{
			if (!ModelState.IsValid)
			{
				bookVM.CategoryList = _unitOfWork.Category.GetAll().Select(
				u => new SelectListItem
				{
					Text = u.CategoryName,
					Value = u.CategoryId.ToString()
				}); // Get all categories and convert them to SelectListItem
					
				
				return View(bookVM);

			}

			_unitOfWork.Book.Add(bookVM.Book);
			_unitOfWork.Save();
			TempData["success"] = "Book created successfully";
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
