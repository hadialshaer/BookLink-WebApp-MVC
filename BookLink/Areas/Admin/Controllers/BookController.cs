using BookLink.DataAccess.Data;
using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using BookLink.Models.ViewModels;
using BookLink.Utility;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BookLink.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
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
				if (bookVM.Book.TransactionType == TransactionType.Lend)
				{

					bookVM.Book.LenderId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
				}
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
				// Remove errors for Lend-specific fields
				ModelState.Remove("Book.BorrowingFee");
				ModelState.Remove("Book.MaxLendDurationDays");

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
				TempData["success"] = "Book updated successfully";
			}
			else
			{
				_unitOfWork.Book.Add(bookVM.Book);
				TempData["success"] = "Book created successfully";
			}

			_unitOfWork.Save();
			
			return RedirectToAction(nameof(Index));

		}

		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
			var books = _unitOfWork.Book.GetAll(includeProperties: "BookCategory")
				.Select(b => new
				{
					b.BookId,
					b.Title,
					b.Author,
					b.ListPrice,
					TransactionType = b.TransactionType.ToString(),
					MaxLendDurationDays = b.TransactionType == TransactionType.Lend ? (int?)b.MaxLendDurationDays : null,
					DueDate = b.TransactionType == TransactionType.Lend && b.DueDate.HasValue
						? b.DueDate.Value.ToString("yyyy-MM-dd") : null,
					Category = new { b.BookCategory.CategoryName },
					BookStatus = b.BookStatus.ToString()
				}).ToList();

			return Json(new { data = books });
		}

		// Export to Excel
		[HttpGet]
		public IActionResult ExportToExcel()
		{
			var books = _unitOfWork.Book.GetAll(includeProperties: "BookCategory").ToList();

			using var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("Books");

			// Add headers
			worksheet.Cell(1, 1).Value = "Title";
			worksheet.Cell(1, 2).Value = "Author";
			worksheet.Cell(1, 3).Value = "Category";
			worksheet.Cell(1, 4).Value = "Transaction Type";
			worksheet.Cell(1, 5).Value = "List Price";
			worksheet.Cell(1, 6).Value = "Price (1-3)";
			worksheet.Cell(1, 7).Value = "Price (3+)";
			worksheet.Cell(1, 8).Value = "Price (5+)";
			worksheet.Cell(1, 9).Value = "Borrowing Fee";
			worksheet.Cell(1, 10).Value = "Max Lend Days";
			worksheet.Cell(1, 11).Value = "Due Date";
			worksheet.Cell(1, 12).Value = "Status";

			// Add data
			int row = 2;
			foreach (var book in books)
			{
				worksheet.Cell(row, 1).Value = book.Title;
				worksheet.Cell(row, 2).Value = book.Author;
				worksheet.Cell(row, 3).Value = book.BookCategory?.CategoryName;
				worksheet.Cell(row, 4).Value = book.TransactionType.ToString();
				worksheet.Cell(row, 5).Value = book.ListPrice;
				worksheet.Cell(row, 6).Value = book.Price;
				worksheet.Cell(row, 7).Value = book.Price3;
				worksheet.Cell(row, 8).Value = book.Price5;
				worksheet.Cell(row, 9).Value = book.BorrowingFee;
				worksheet.Cell(row, 10).Value = book.MaxLendDurationDays;
				worksheet.Cell(row, 11).Value = book.DueDate?.ToString("yyyy-MM-dd");
				worksheet.Cell(row, 12).Value = book.BookStatus.ToString();
				row++;
			}

			using var stream = new MemoryStream();
			workbook.SaveAs(stream);
			var content = stream.ToArray();

			return File(
				content,
				"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
				$"Books_Export_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
		}

		// Delete book
		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var bookToDelete = _unitOfWork.Book.Get(u => u.BookId == id);

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

			// Delete related borrow requests first
			var borrowRequests = _unitOfWork.BorrowRequest.GetAll(b => b.BookId == id);
			foreach (var request in borrowRequests)
			{
				_unitOfWork.BorrowRequest.Remove(request);
			}

			_unitOfWork.Book.Remove(bookToDelete);
			_unitOfWork.Save();

			return Json(new { success = true, message = "Delete successful" });
		}
		#endregion


	}
}
