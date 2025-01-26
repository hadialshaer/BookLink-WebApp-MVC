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
		private readonly IWebHostEnvironment _webHostEnvironment; // Used to get the web root path
		public BookController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;

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

			string wwwRootPath = _webHostEnvironment.WebRootPath; // Get the root path

			if (file != null)
			{
				string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
				string bookPath = Path.Combine(wwwRootPath, @"images\books");

				if(!string.IsNullOrEmpty(bookVM.Book.ImageUrl))
				{
					// delete the old image
					string oldImagePath = Path.Combine(wwwRootPath, bookVM.Book.ImageUrl.TrimStart('\\'));

					if (System.IO.File.Exists(oldImagePath))
					{
						System.IO.File.Delete(oldImagePath);
					}
				}

				using (var fileStream = new FileStream(Path.Combine(bookPath, fileName), FileMode.Create))
				{
					file.CopyTo(fileStream);
				}

				bookVM.Book.ImageUrl = @"\images\books\" + fileName;

			}

			if(bookVM.Book.BookId != 0)
			{
				_unitOfWork.Book.Update(bookVM.Book);
			}
			else
			{
				_unitOfWork.Book.Add(bookVM.Book);
			}

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
