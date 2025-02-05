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
			List<Book> books = _unitOfWork.Book.GetAll(includeProperties:"BookCategory").ToList();
			return View(books);
		}


		public ActionResult Upsert(int? id, TransactionType? transactionType)
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
				bookVM.Book.TransactionType = transactionType ?? TransactionType.Sell;
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
			if (bookVM.Book.TransactionType == TransactionType.Sell)
			{
				// Selling-specific validation
				if (bookVM.Book.ListPrice == null || bookVM.Book.ListPrice <= 0)
				{
					ModelState.AddModelError("Book.ListPrice",
						"List price is required for selling");
				}

				if (bookVM.Book.Price == null || bookVM.Book.Price <= 0)
				{
					ModelState.AddModelError("Book.Price",
						"Price for 1-3 is required for selling");
				}

				if (bookVM.Book.Price3 == null || bookVM.Book.Price3 <= 0)
				{
					ModelState.AddModelError("Book.Price3",
						"Price for 3+ is required for selling");
				}

				if (bookVM.Book.Price5 == null || bookVM.Book.Price5 <= 0)
				{
					ModelState.AddModelError("Book.Price5",
						"Price for 5+ is required for selling");
				}

			}
			else
			{
				// Lending-specific validation
				if (bookVM.Book.MaxLendDurationDays == null)
				{
					ModelState.AddModelError("Book.MaxLendDurationDays",
						"Max duration is required for lending");
				}

				if (bookVM.Book.BookStatus == BookStatus.Borrowed &&
					!bookVM.Book.DueDate.HasValue)
				{
					ModelState.AddModelError("Book.DueDate",
						"Due date is required for borrowed books");
				}
			}


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
					string oldImagePath =
										Path.Combine(wwwRootPath, bookVM.Book.ImageUrl.TrimStart('\\'));

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

		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
			var books = _unitOfWork.Book.GetAll(includeProperties: "BookCategory")
				.Select(b => new {
					b.BookId,
					b.Title,
					b.Author,
					b.ListPrice,
					TransactionType = b.TransactionType.ToString(),
					MaxLendDurationDays = b.TransactionType == TransactionType.Lend ? b.MaxLendDurationDays : null,
					DueDate = b.TransactionType == TransactionType.Lend && b.DueDate.HasValue
						? b.DueDate.Value.ToString("yyyy-MM-dd") : null,
					Category = new { b.BookCategory.CategoryName }
				}).ToList();

			return Json(new { data = books });
		}

		// Delete book
		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			Book bookToDelete = _unitOfWork.Book.Get(u => u.BookId == id);

			if (bookToDelete == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

			var oldImagePath =	
							Path.Combine(_webHostEnvironment.WebRootPath,
							bookToDelete.ImageUrl.TrimStart('\\'));

			if (System.IO.File.Exists(oldImagePath))
			{
				System.IO.File.Delete(oldImagePath);
			}

			_unitOfWork.Book.Remove(bookToDelete);
			_unitOfWork.Save();

			return Json(new { success = true, message = "Delete successful" });
		}
		#endregion


	}
}
