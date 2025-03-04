using Microsoft.AspNetCore.Mvc;
using BookLink.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using BookLink.Models;
using Microsoft.AspNetCore.Authorization;
using BookLink.Utility;

namespace BookLink.Areas.Member.Controllers
{
	[Area("Member")]
	[Authorize(Roles = SD.Role_Member)]
	public class WishlistController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<User> _userManager;

		public WishlistController(IUnitOfWork unitOfWork, UserManager<User> userManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
		}

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
				var wishlistItems = _unitOfWork.Wishlist.GetAll(
					w => w.UserId == userId,
					includeProperties: "Book"
				);
				return View(wishlistItems);
			}
			catch (Exception ex)
			{
				TempData["error"] = $"Error loading wishlist: {ex.Message}";
				return RedirectToAction("Index", "Home");
			}
		}

		[HttpPost]
		public IActionResult ToggleWishlist(int bookId)
		{
			try
			{
				var userId = _userManager.GetUserId(User);
				if (string.IsNullOrEmpty(userId))
				{
					return Json(new { success = false, error = "User not authenticated." });
				}
				var wishlistItem = _unitOfWork.Wishlist.Get(
					w => w.UserId == userId && w.BookId == bookId
				);
				int currentCount = HttpContext.Session.GetInt32(SD.SessionWishlist) ?? _unitOfWork.Wishlist.GetAll(u => u.UserId == userId).Count();
				if (wishlistItem != null)
				{
					_unitOfWork.Wishlist.Remove(wishlistItem);
					_unitOfWork.Save();
					currentCount--;
					HttpContext.Session.SetInt32(SD.SessionWishlist, currentCount);
					TempData["success"] = "Book removed from wishlist."; // Set TempData for page loads
					return Json(new { success = true, inWishlist = false, count = currentCount, message = TempData["success"] });
				}
				else
				{
					var newWishlistItem = new Wishlist
					{
						UserId = userId,
						BookId = bookId,
						AddedDate = DateTime.UtcNow
					};
					_unitOfWork.Wishlist.Add(newWishlistItem);
					_unitOfWork.Save();
					currentCount++;
					HttpContext.Session.SetInt32(SD.SessionWishlist, currentCount);
					TempData["success"] = "Book saved to wishlist."; // Updated message for consistency
					return Json(new { success = true, inWishlist = true, count = currentCount, message = TempData["success"] });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, error = $"Error toggling wishlist: {ex.Message}" });
			}
		}

		[HttpPost]
		public IActionResult RemoveFromWishlist(int bookId)
		{
			try
			{
				var userId = _userManager.GetUserId(User);
				if (string.IsNullOrEmpty(userId))
				{
					return Json(new { success = false, error = "User not authenticated." });
				}
				var wishlistItem = _unitOfWork.Wishlist.Get(
					w => w.UserId == userId && w.BookId == bookId
				);
				if (wishlistItem != null)
				{
					_unitOfWork.Wishlist.Remove(wishlistItem);
					_unitOfWork.Save();
					int currentCount = HttpContext.Session.GetInt32(SD.SessionWishlist) ?? _unitOfWork.Wishlist.GetAll(u => u.UserId == userId).Count();
					currentCount--;
					HttpContext.Session.SetInt32(SD.SessionWishlist, currentCount);
					TempData["success"] = "Book removed from wishlist.";
					return Json(new { success = true, count = currentCount, message = TempData["success"] });
				}
				return Json(new { success = false, error = "Book not found in wishlist." });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, error = $"Error removing from wishlist: {ex.Message}" });
			}
		}
	}
}