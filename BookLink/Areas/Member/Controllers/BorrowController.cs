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
			var userId = _userManager.GetUserId(User);
			var requests = _unitOfWork.BorrowRequest.GetAll(
				br => br.LenderId == userId || br.BorrowerId == userId,
				includeProperties: "Book,Lender,Borrower"
			);
			return View(requests);
		}

		[HttpPost]
		public IActionResult Approve(int id)
		{
			var request = _unitOfWork.BorrowRequest.Get(br => br.Id == id);
			var book = _unitOfWork.Book.Get(b => b.BookId == request.BookId);

			if (book == null || request == null)
			{
				TempData["error"] = "The book or request could not be found.";
				return NotFound();
			}
				

			book.BookStatus = BookStatus.Borrowed;
			request.Status = BorrowRequestStatus.Approved;
			request.ApprovalDate = DateTime.Now;
			request.DueDate = DateTime.Now.AddDays(book.MaxLendDurationDays ?? 14);

			_unitOfWork.Book.Update(book);
			_unitOfWork.BorrowRequest.Update(request);
			_unitOfWork.Save();
			TempData["success"] = "The borrow request has been approved successfully.";

			return RedirectToAction(nameof(Index));
		}
	}
}
