using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using BookLink.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookLink.ViewComponents
{
	public class WishlistViewComponent : ViewComponent
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<User> _userManager;

		public WishlistViewComponent(IUnitOfWork unitOfWork, UserManager<User> userManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var count = HttpContext.Session.GetInt32(SD.SessionWishlist);
			var userId = _userManager.GetUserId((System.Security.Claims.ClaimsPrincipal)User);

			if (userId != null)
			{
				if (count == null)
				{
					var wishlistCount = _unitOfWork.Wishlist.GetAll(u => u.UserId == userId).Count();
					HttpContext.Session.SetInt32(SD.SessionWishlist, wishlistCount);
					return View(wishlistCount);
				}
				return View(count);
			}
			else
			{
				HttpContext.Session.Clear();
				return View(0);
			}
		}
	}
}