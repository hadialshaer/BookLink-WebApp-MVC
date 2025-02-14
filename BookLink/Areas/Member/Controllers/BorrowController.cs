using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using BookLink.Models.ViewModels;
using BookLink.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookLink.Areas.Member.Controllers
{
	[Area("Member")]
	[Authorize(Roles = SD.Role_Member)]
	public class BorrowController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<User> _userManager;

		public BorrowController(IUnitOfWork unitOfWork, UserManager<User> userManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
		}

		// Show Borrowing Requests Details
		public IActionResult Index()
		{
			try
			{
				var userId = _userManager.GetUserId(User);

				if (userId == null)
				{
					TempData["error"] = "User is not authenticated.";
					return RedirectToAction("Index", "Home");
				}

				var borrowRequests = _unitOfWork.BorrowRequest.GetAll(
					r => r.BorrowerId == userId || r.LenderId == userId,
					includeProperties: "Book,Lender,Location,Borrower"
				);

				return View(borrowRequests);

			}
			catch (Exception ex)
			{
				TempData["error"] = $"Error: {ex.Message}";
				return RedirectToAction("Index", "Home");
			}
		}

		// Get Request Borrow
		[HttpGet]
		public IActionResult RequestBorrow(int bookId)
		{
			try
			{
				var book = _unitOfWork.Book.Get(b => b.BookId == bookId,
					includeProperties:"BookCategory,Lender");


				if (book == null || book.BookStatus != BookStatus.Available)
				{
					TempData["error"] = "Book not found";
					return RedirectToAction(nameof(Index));
				}

				// Ensure critical navigation properties exist
				if (book.Lender == null || book.BookCategory == null)
				{
					TempData["error"] = "Book information is incomplete";
					return RedirectToAction(nameof(Index));
				}

				var borrowRequestVM = new BorrowRequestVM()
				{
					BorrowRequest = new BorrowRequest
					{
						BookId = bookId,
						Book = book,
						LenderId = book.LenderId
					},
					LocationList = _unitOfWork.Location.GetAll().Select(l => new SelectListItem
					{
						Text = l.Name,
						Value = l.Id.ToString()
					})
				};

				// Check for existing pending request
				var userId = _userManager.GetUserId(User);
				var existingRequest = _unitOfWork.BorrowRequest.Get(r =>
					r.BookId == bookId &&
					r.BorrowerId == userId &&
					r.Status == BorrowRequestStatus.Pending);

				if (existingRequest != null)
				{
					TempData["error"] = "You already have a pending request for this book";
					return RedirectToAction(nameof(Index));
				}

				return PartialView("_CreateBorrowRequest", borrowRequestVM);
			}
			catch (Exception ex)
			{
				TempData["error"] = $"Error: {ex.Message}";
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult SubmitBorrowRequest(BorrowRequestVM borrowRequestVM)
		{
			try
			{

				if (!ModelState.IsValid)
				{	
					// RELOAD BOOK DATA
					borrowRequestVM.BorrowRequest.Book = _unitOfWork.Book.Get(
						b => b.BookId == borrowRequestVM.BorrowRequest.BookId,
						includeProperties: "BookCategory,Lender"
					);

					// RELOAD LOCATIONS
					borrowRequestVM.LocationList = _unitOfWork.Location.GetAll()
						.Select(l => new SelectListItem
						{
							Text = l.Name,
							Value = l.Id.ToString()
						});

					return PartialView("_CreateBorrowRequest", borrowRequestVM);
				}


				var book = _unitOfWork.Book.Get(b => b.BookId == borrowRequestVM.BorrowRequest.BookId,
					includeProperties: "Lender,BookCategory");
				var userId = _userManager.GetUserId(User);

				if (borrowRequestVM?.BorrowRequest == null)
				{
					TempData["error"] = "Invalid request data";
					return RedirectToAction(nameof(Index));
				}
				

				if (book == null || book.BookStatus != BookStatus.Available)
				{
					return NotFound();
				}

				// Check for existing pending request
				var existingRequest = _unitOfWork.BorrowRequest.Get(br =>
					br.BookId == borrowRequestVM.BorrowRequest.BookId &&
					br.BorrowerId == userId &&
					br.Status == BorrowRequestStatus.Pending);

				if (existingRequest != null)
				{
					ModelState.AddModelError("", "You already have a pending request for this book.");
					return PartialView("_CreateBorrowRequest", borrowRequestVM);
				}

				if (book.LenderId == userId)

				{
					ModelState.AddModelError(string.Empty, "You cannot borrow your own book.");
					// Reload data to return the partial view
					borrowRequestVM.BorrowRequest.Book = book;
					borrowRequestVM.LocationList = _unitOfWork.Location.GetAll()
						.Select(l => new SelectListItem { Text = l.Name, Value = l.Id.ToString() });

					return PartialView("_CreateBorrowRequest", borrowRequestVM);
				}

				// Populate required fields
				borrowRequestVM.BorrowRequest.BorrowerId = userId;
				borrowRequestVM.BorrowRequest.LenderId = book.LenderId;
				borrowRequestVM.BorrowRequest.RequestDate = DateTime.UtcNow;
				borrowRequestVM.BorrowRequest.Status = BorrowRequestStatus.Pending;
				borrowRequestVM.BorrowRequest.DueDate = DateTime.UtcNow.AddDays(book.MaxLendDurationDays);

				// Update book status
				book.BookStatus = BookStatus.Pending;
				_unitOfWork.Book.Update(book);

				_unitOfWork.BorrowRequest.Add(borrowRequestVM.BorrowRequest);
				_unitOfWork.Save();

				TempData["success"] = "Borrow request submitted successfully!";
				return Json(new { success = true, redirect = Url.Action("Index") });
			}
			catch (Exception ex)
			{
				TempData["error"] = $"Error: {ex.Message}";
				return RedirectToAction(nameof(Index));
			}
		}
		[HttpPost]
		public IActionResult ApproveRequest(int requestId)
		{
			try
			{
				var borrowRequest = _unitOfWork.BorrowRequest.Get(r => r.Id == requestId);
				var userId = _userManager.GetUserId(User);


				if (borrowRequest == null)
				{
					TempData["error"] = "Request not found";
					return RedirectToAction(nameof(Index));
				}

				var book = _unitOfWork.Book.Get(b => b.BookId == borrowRequest.BookId);

				if (book == null)
				{
					return Json(new { success = false, error = "Book not found" });
				}

				if (book.BookStatus != BookStatus.Available)
				{
					TempData["error"] = "Book is no longer available";
					return RedirectToAction(nameof(Index));
				}

				if (borrowRequest.LenderId != userId)
				{
					TempData["error"] = "Unauthorized action";
					return RedirectToAction(nameof(Index));
				}

				// Update book status
				book.BookStatus = BookStatus.Borrowed;
				book.BorrowerId = borrowRequest.BorrowerId;
				book.DueDate = borrowRequest.DueDate;
				_unitOfWork.Book.Update(book);


				// Update request
				_unitOfWork.BorrowRequest.UpdateStatus(requestId, BorrowRequestStatus.Approved);
				_unitOfWork.BorrowRequest.Update(borrowRequest);

				// Find and reject all other pending requests for the same book
				var otherPendingRequests = _unitOfWork.BorrowRequest.GetAll(r =>
					r.BookId == borrowRequest.BookId &&
					r.Status == BorrowRequestStatus.Pending &&
					r.Id != requestId);

				foreach (var pendingRequest in otherPendingRequests)
				{
					_unitOfWork.BorrowRequest.UpdateStatus(pendingRequest.Id, BorrowRequestStatus.Rejected);
				}

				_unitOfWork.Save();

				TempData["success"] = "Request approved successfully";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["error"] = $"Error approving request: {ex}";
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpPost]
		public IActionResult RejectRequest(int requestId)
		{
			try
			{
				var borrowRequest = _unitOfWork.BorrowRequest.Get(r => r.Id == requestId);
				var userId = _userManager.GetUserId(User);

				if (borrowRequest == null)
				{
					TempData["error"] = "Request not found";
					return RedirectToAction(nameof(Index));
				}

				var book = _unitOfWork.Book.Get(b => b.BookId == borrowRequest.BookId);

				if (book == null)
				{
					return Json(new { success = false, error = "Book not found" });
				}

				if (book.BookStatus != BookStatus.Available)
				{
					TempData["error"] = "Book is no longer available";
					return RedirectToAction(nameof(Index));
				}

				if (borrowRequest.LenderId != userId)
				{
					TempData["error"] = "Unauthorized action";
					return RedirectToAction(nameof(Index));
				}


				_unitOfWork.BorrowRequest.UpdateStatus(requestId, BorrowRequestStatus.Rejected);
				_unitOfWork.Save();

				TempData["success"] = "Request rejected successfully";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["error"] = $"Error rejecting request: {ex}";
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpPost]
		public IActionResult ReturnBook(int requestId)
		{
			try
			{
				ModelState.Remove("Location s");
				ModelState.Remove("BorrowRequest.Location");
				ModelState.Remove("BorrowRequest.Location.Name");

				var borrowRequest = _unitOfWork.BorrowRequest.Get(r => r.Id == requestId);
				var userId = _userManager.GetUserId(User);

				if (borrowRequest == null)
				{
					TempData["error"] = "Request not found";
					return RedirectToAction(nameof(Index));
				}

				var book = _unitOfWork.Book.Get(b => b.BookId == borrowRequest.BookId);

				if (book == null)
				{
					TempData["error"] = "Book not found";
					return RedirectToAction(nameof(Index));
				}

				if (book.BookStatus == BookStatus.Available)
				{
					TempData["error"] = "This book is already available (returned).";
					return RedirectToAction(nameof(Index));
				}

				if (borrowRequest.BorrowerId != userId && borrowRequest.LenderId != userId)
				{
					TempData["error"] = "Unauthorized return attempt";
					return RedirectToAction(nameof(Index));
				}

				// Update book status
				book.BookStatus = BookStatus.Available;
				book.BorrowerId = null;
				book.DueDate = null;
				_unitOfWork.Book.Update(book);

				// Update request
				_unitOfWork.BorrowRequest.UpdateStatus(requestId, BorrowRequestStatus.Returned);
				_unitOfWork.Save();

				TempData["success"] = "Book marked as returned";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["error"] = $"Error processing return {ex}";
				return RedirectToAction(nameof(Index));
			}
		}
	}

}