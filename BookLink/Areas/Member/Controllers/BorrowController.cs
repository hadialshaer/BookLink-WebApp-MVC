using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using BookLink.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

		public IActionResult Index()
		{
			try
			{
				var userId = _userManager.GetUserId(User);
				var requests = _unitOfWork.BorrowRequest.GetAll(
					r => r.BorrowerId == userId || r.LenderId == userId,
					includeProperties: "Book,Borrower,Lender"
				);
				return View(requests);
			}
			catch (Exception ex)
			{
				TempData["error"] = "Error loading borrow requests";
				return RedirectToAction("Index", "Home");
			}
		}

		[HttpPost]
		public IActionResult RequestBorrow(int bookId)
		{
			try
			{
				var book = _unitOfWork.Book.Get(b => b.BookId == bookId);
				if (book == null)
				{
					TempData["error"] = "Book not found";
					return RedirectToAction(nameof(Index));
				}

				if (book.TransactionType != TransactionType.Lend)
				{
					TempData["error"] = "This book is not available for borrowing";
					return RedirectToAction(nameof(Index));
				}

				if (book.BookStatus != BookStatus.Available)
				{
					TempData["error"] = "This book is currently not available";
					return RedirectToAction(nameof(Index));
				}

				var existingRequest = _unitOfWork.BorrowRequest.Get(r =>
					r.BookId == bookId &&
					r.BorrowerId == _userManager.GetUserId(User) &&
					r.Status == BorrowRequestStatus.Pending);

				if (existingRequest != null)
				{
					TempData["error"] = "You already have a pending request for this book";
					return RedirectToAction(nameof(Index));
				}

				var request = new BorrowRequest
				{
					BookId = bookId,
					BorrowerId = _userManager.GetUserId(User),
					LenderId = book.LenderId,
					Status = BorrowRequestStatus.Pending,
					RequestDate = DateTime.Now
				};

				_unitOfWork.BorrowRequest.Add(request);
				_unitOfWork.Save();

				TempData["success"] = "Borrow request submitted successfully";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["error"] = "Error processing borrow request";
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpPost]
		public IActionResult ApproveRequest(int id)
		{
			try
			{
				var request = _unitOfWork.BorrowRequest.Get(r => r.Id == id);
				if (request == null)
				{
					TempData["error"] = "Request not found";
					return RedirectToAction(nameof(Index));
				}

				var book = _unitOfWork.Book.Get(b => b.BookId == request.BookId);
				if (book.BookStatus != BookStatus.Available)
				{
					TempData["error"] = "Book is no longer available";
					return RedirectToAction(nameof(Index));
				}

				// Update book status
				book.BookStatus = BookStatus.Borrowed;
				book.BorrowerId = request.BorrowerId;
				book.DueDate = DateTime.Now.AddDays(14); // 2 weeks borrowing period

				// Update request
				request.Status = BorrowRequestStatus.Approved;
				request.ApprovalDate = DateTime.Now;
				_unitOfWork.BorrowRequest.Update(request);
				_unitOfWork.Save();

				TempData["success"] = "Request approved successfully";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["error"] = "Error approving request";
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpPost]
		public IActionResult RejectRequest(int id)
		{
			try
			{
				var request = _unitOfWork.BorrowRequest.Get(r => r.Id == id);
				if (request == null)
				{
					TempData["error"] = "Request not found";
					return RedirectToAction(nameof(Index));
				}

				request.Status = BorrowRequestStatus.Rejected;
				_unitOfWork.BorrowRequest.Update(request);
				_unitOfWork.Save();

				TempData["success"] = "Request rejected successfully";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["error"] = "Error rejecting request";
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpPost]
		public IActionResult ReturnBook(int id)
		{
			try
			{
				var request = _unitOfWork.BorrowRequest.Get(r => r.Id == id);
				if (request == null)
				{
					TempData["error"] = "Request not found";
					return RedirectToAction(nameof(Index));
				}

				var book = _unitOfWork.Book.Get(b => b.BookId == request.BookId);
				if (book == null)
				{
					TempData["error"] = "Book not found";
					return RedirectToAction(nameof(Index));
				}

				// Update book status
				book.BookStatus = BookStatus.Available;
				book.BorrowerId = null;
				book.DueDate = null;

				// Update request
				request.Status = BorrowRequestStatus.Returned;
				request.ReturnDate = DateTime.Now;
				_unitOfWork.BorrowRequest.Update(request);
				_unitOfWork.Save();

				TempData["success"] = "Book marked as returned";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["error"] = "Error processing return";
				return RedirectToAction(nameof(Index));
			}
		}
	}
}
