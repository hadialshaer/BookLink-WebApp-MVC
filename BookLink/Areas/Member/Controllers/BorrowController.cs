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

		/// <summary>
		/// Displays borrowing requests for the current user.
		/// </summary>
		public IActionResult Index()
		{
			try
			{
				var userId = _userManager.GetUserId(User);
				if (string.IsNullOrEmpty(userId))
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

		/// <summary>
		/// GET: Returns the partial view for creating a borrow request for a specific book.
		/// </summary>
		[HttpGet]
		public IActionResult RequestBorrow(int bookId)
		{
			try
			{
				// Retrieve the book along with its category and lender.
				var book = _unitOfWork.Book.Get(
					b => b.BookId == bookId,
					includeProperties: "BookCategory,Lender"
				);

				if (book == null || book.BookStatus != BookStatus.Available)
					return Json(new { success = false, error = "Book not found or not available" });

				// Ensure critical navigation properties exist.
				if (book.Lender == null || book.BookCategory == null)
					return Json(new { success = false, error = "Critical data missing" });

				// Prepare the view model for the borrow request.
				var borrowRequestVM = new BorrowRequestVM()
				{
					BorrowRequest = new BorrowRequest
					{
						BookId = bookId,
						Book = book,
						LenderId = book.LenderId
					},
					LocationList = _unitOfWork.Location.GetAll()
						.Select(l => new SelectListItem
						{
							Text = l.Name,
							Value = l.Id.ToString()
						})
				};

				// Get the current user.
				var userId = _userManager.GetUserId(User);
				if (string.IsNullOrEmpty(userId))
					return Json(new { success = false, error = "User not authenticated, Please Log In" });

				// Check for an existing pending request for this book.
				var existingRequest = _unitOfWork.BorrowRequest.Get(br =>
					br.BookId == borrowRequestVM.BorrowRequest.BookId &&
					br.BorrowerId == userId &&
					br.Status == BorrowRequestStatus.Pending
				);

				if (existingRequest != null)
					return Json(new { success = false, error = "You already have a pending request for this book." });

				// Prevent users from borrowing their own book.
				if (book.LenderId == userId)
					return Json(new { success = false, error = "You cannot borrow your own book." });

				// Return the partial view for creating the borrow request.
				return PartialView("_CreateBorrowRequest", borrowRequestVM);
			}
			catch (Exception ex)
			{
				TempData["error"] = $"Error: {ex.Message}";
				return RedirectToAction("Details", "Home");
			}
		}

		/// <summary>
		/// POST: Submits a new borrow request.
		/// </summary>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult SubmitBorrowRequest(BorrowRequestVM borrowRequestVM)
		{
			try
			{
				// Validate the model.
				if (!ModelState.IsValid)
				{
					// Reload book data.
					borrowRequestVM.BorrowRequest.Book = _unitOfWork.Book.Get(
						b => b.BookId == borrowRequestVM.BorrowRequest.BookId,
						includeProperties: "BookCategory,Lender"
					);
					// Reload location list.
					borrowRequestVM.LocationList = _unitOfWork.Location.GetAll()
						.Select(l => new SelectListItem
						{
							Text = l.Name,
							Value = l.Id.ToString()
						});
					return PartialView("_CreateBorrowRequest", borrowRequestVM);
				}

				// Retrieve the book details.
				var book = _unitOfWork.Book.Get(
					b => b.BookId == borrowRequestVM.BorrowRequest.BookId,
					includeProperties: "Lender,BookCategory"
				);
				var userId = _userManager.GetUserId(User);

				// Check that the borrow request object exists.
				if (borrowRequestVM?.BorrowRequest == null)
				{
					TempData["error"] = "Invalid request data";
					return RedirectToAction("Details", "Home");
				}

				if (book == null || book.BookStatus != BookStatus.Available)
					return NotFound();

				// Assign the current user's ID as the BorrowerId.
				borrowRequestVM.BorrowRequest.BorrowerId = userId;
				// Populate other required fields.
				borrowRequestVM.BorrowRequest.LenderId = book.LenderId;
				borrowRequestVM.BorrowRequest.RequestDate = DateTime.UtcNow;
				borrowRequestVM.BorrowRequest.Status = BorrowRequestStatus.Pending;
				borrowRequestVM.BorrowRequest.DueDate = DateTime.UtcNow.AddDays(book.MaxLendDurationDays);

				// Update the book status to Pending.
				book.BookStatus = BookStatus.Pending;
				_unitOfWork.Book.Update(book);

				// Add the new borrow request.
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

		/// <summary>
		/// POST: Approves a borrow request.
		/// </summary>
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
					return Json(new { success = false, error = "Book not found" });

				// Allow approval only if the book is currently pending.
				if (book.BookStatus != BookStatus.Pending)
				{
					TempData["error"] = "Book is not in a state that can be approved.";
					return RedirectToAction(nameof(Index));
				}

				// Only the lender is allowed to approve the request.
				if (borrowRequest.LenderId != userId)
				{
					TempData["error"] = "Unauthorized action";
					return RedirectToAction(nameof(Index));
				}

				// Update the book's status to Borrowed.
				book.BookStatus = BookStatus.Borrowed;
				// Set the BorrowerId from the borrow request (should have been set during submission).
				book.BorrowerId = borrowRequest.BorrowerId;
				book.DueDate = borrowRequest.DueDate;
				_unitOfWork.Book.Update(book);

				// Update the borrow request status to Approved.
				_unitOfWork.BorrowRequest.UpdateStatus(requestId, BorrowRequestStatus.Approved);

				// Reject all other pending requests for the same book.
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

		/// <summary>
		/// POST: Rejects a borrow request.
		/// </summary>
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
					return Json(new { success = false, error = "Book not found" });

				// Check if the book is available (should be available for rejection).
				if (book.BookStatus != BookStatus.Available)
				{
					TempData["error"] = "Book is no longer available";
					return RedirectToAction(nameof(Index));
				}

				// Only the lender can reject a request.
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

		/// <summary>
		/// POST: Marks a book as returned.
		/// </summary>
		[HttpPost]
		public IActionResult ReturnBook(int requestId)
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
					TempData["error"] = "Book not found";
					return RedirectToAction(nameof(Index));
				}

				if (book.BookStatus == BookStatus.Available)
				{
					TempData["error"] = "This book is already available (returned).";
					return RedirectToAction(nameof(Index));
				}

				// Only the borrower or lender can mark the book as returned.
				if (borrowRequest.BorrowerId != userId && borrowRequest.LenderId != userId)
				{
					TempData["error"] = "Unauthorized return attempt";
					return RedirectToAction(nameof(Index));
				}

				// Update the book status to Available.
				book.BookStatus = BookStatus.Available;
				book.BorrowerId = null;
				book.DueDate = null;
				_unitOfWork.Book.Update(book);

				// Update the borrow request's status to Returned.
				_unitOfWork.BorrowRequest.UpdateStatus(requestId, BorrowRequestStatus.Returned);
				_unitOfWork.Save();

				TempData["success"] = "Book marked as returned";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["error"] = $"Error processing return: {ex}";
				return RedirectToAction(nameof(Index));
			}
		}
	}
}
